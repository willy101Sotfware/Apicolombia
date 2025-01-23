using System.Collections.Generic;
using System.Windows;

namespace ApiColombia
{
    public partial class AttractionDetailWindow : Window
    {
        public AttractionDetailWindow(List<TouristicAttraction> attractions)
        {
            InitializeComponent();
            AttractionsContainer.ItemsSource = attractions;
        }
    }
}