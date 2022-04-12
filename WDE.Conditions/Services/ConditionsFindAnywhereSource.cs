using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Prism.Commands;
using WDE.Common.Database;
using WDE.Common.Services.FindAnywhere;
using WDE.Common.Services.MessageBox;
using WDE.Common.Types;
using WDE.Common.Utils;
using WDE.Conditions.Data;
using WDE.Module.Attributes;
using WDE.SqlQueryGenerator;

namespace WDE.Conditions.Services;

[AutoRegister]
public class ConditionsFindAnywhereSource : IFindAnywhereSource
{
    private readonly IConditionDataManager dataManager;
    private readonly IMySqlExecutor mySqlExecutor;
    private readonly Lazy<IMessageBoxService> messageBoxService;

    public ConditionsFindAnywhereSource(IConditionDataManager dataManager,
        IMySqlExecutor mySqlExecutor,
        Lazy<IMessageBoxService> messageBoxService)
    {
        this.dataManager = dataManager;
        this.mySqlExecutor = mySqlExecutor;
        this.messageBoxService = messageBoxService;
    }
    
    public async Task Find(IFindAnywhereResultContext resultContext, IReadOnlyList<string> parameterName, long parameterValue, CancellationToken cancellationToken)
    {
        var command = new DelegateCommand(() =>
        {
            messageBoxService.Value.ShowDialog(new MessageBoxFactory<bool>()
                .SetTitle("Operation not supported yet")
                .SetMainInstruction("Conditions not yet supported")
                .SetContent("Sorry, opening conditions directly is not yet supported")
                .WithButton("Sorry again", false)
                .Build()).ListenErrors();
        });
        var table = Queries.Table("conditions");
        var where = table.ToWhere();
        foreach (var cond in dataManager.AllConditionData)
        {
            if (cond.Parameters == null)
                continue;

            for (int i = 0; i < cond.Parameters.Count; ++i)
            {
                if (parameterName.IndexOf(cond.Parameters[i].Type) == -1)
                    continue;

                var colName = "ConditionValue" + (i + 1);

                where = where.OrWhere(row => row.Column<int>("ConditionTypeOrReference") == cond.Id &&
                                             row.Column<long>(colName) == parameterValue);
            }
        }

        var result = await mySqlExecutor.ExecuteSelectSql(where.Select().QueryString);
        foreach (var row in result)
        {
            resultContext.AddResult(new FindAnywhereResult(new ImageUri("Icons/document_conditions.png"),
                "Condition " + row["Comment"].Item2,
                 string.Join(", ", row.Select(pair => pair.Key + ": " + pair.Value.Item2)),
                null,
                command));
        }
    }
}