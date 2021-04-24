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
    /// Interaction logic for TableauControl.xaml
    /// </summary>
    public partial class TableauControl : UserControl
    {
        public TableauControl()
        {
            InitializeComponent();
        }

        public Tableau Tableau
        {
            get { return (Tableau)GetValue(CardProperty); }
            set { SetValue(CardProperty, value); }
        }


        public static readonly DependencyProperty CardProperty =
            DependencyProperty.Register("Tableau", typeof(Tableau), typeof(TableauControl));


    }
}
