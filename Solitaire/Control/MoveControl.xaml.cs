using Solitaire.Game;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Solitaire.Control
{
    /// <summary>
    /// Interaction logic for MoveControl.xaml
    /// </summary>
    public partial class MoveControl : UserControl
    {
        public MoveControl()
        {
            InitializeComponent();
        }

        public Move Move
        {
            get { return (Move)GetValue(MoveProperty); }
            set { SetValue(MoveProperty, value); }
        }


        public static readonly DependencyProperty MoveProperty =
            DependencyProperty.Register("Move", typeof(Move), typeof(MoveControl));

    }
}
