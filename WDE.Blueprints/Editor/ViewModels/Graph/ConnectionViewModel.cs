﻿using System;
using System.Windows;
using Prism.Mvvm;

namespace WDE.Blueprints.Editor.ViewModels
{
    public class ConnectionViewModel : BindableBase
    {
        private OutputConnectorViewModel @from;

        private Point fromPosition;

        private bool isSelected;

        private InputConnectorViewModel to;

        private Point toPosition;

        public ConnectionViewModel(OutputConnectorViewModel from, InputConnectorViewModel to)
        {
            From = from;
            To = to;
        }

        public ConnectionViewModel(InputConnectorViewModel to)
        {
            To = to;
        }

        public ConnectionViewModel(OutputConnectorViewModel from)
        {
            From = from;
        }

        public OutputConnectorViewModel From
        {
            get => @from;
            set
            {
                if (@from != null)
                {
                    @from.PositionChanged -= OnFromPositionChanged;
                    @from.Connections.Remove(this);
                }

                @from = value;

                if (@from != null)
                {
                    @from.PositionChanged += OnFromPositionChanged;
                    @from.Connections.Add(this);
                    FromPosition = value.Position;
                    To = to;
                }

                RaisePropertyChanged();
            }
        }

        public InputConnectorViewModel To
        {
            get => to;
            set
            {
                if (to != null)
                {
                    to.PositionChanged -= OnToPositionChanged;
                    to.Connection = null;
                }

                to = value;

                if (to != null)
                {
                    to.PositionChanged += OnToPositionChanged;
                    to.Connection = this;
                    ToPosition = to.Position;
                }

                RaisePropertyChanged();
            }
        }

        public Point FromPosition
        {
            get => fromPosition;
            set
            {
                fromPosition = value;
                RaisePropertyChanged();
            }
        }

        public Point ToPosition
        {
            get => toPosition;
            set
            {
                toPosition = value;
                RaisePropertyChanged();
            }
        }

        public bool IsSelected
        {
            get => isSelected;
            set
            {
                isSelected = value;
                RaisePropertyChanged();
            }
        }

        public void Detach()
        {
            if (From != null)
                From.Connections.Remove(this);

            if (To != null)
                To.Connection = null;
        }

        private void OnFromPositionChanged(object sender, EventArgs e)
        {
            FromPosition = From.Position;
        }

        private void OnToPositionChanged(object sender, EventArgs e)
        {
            ToPosition = To.Position;
        }
    }
}