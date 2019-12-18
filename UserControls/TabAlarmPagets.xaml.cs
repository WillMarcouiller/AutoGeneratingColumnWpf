using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace Surveillance {
    /// <summary>
    /// Interaction logic for TabGestionPagets.xaml
    /// </summary>
    public partial class TabAlarmPagets : UserControl {
        public TabAlarmPagets() { InitializeComponent(); }

        private void CboGroupeTraitements_SelectionChanged(object sender, SelectionChangedEventArgs e) { LoadProcessesAlarmLevels(); }
        private void DgAlarmesTraitements_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e) {
            // Whenever commented out, image file name is displayed in cells.
            // When uncommented, no image nor image file names displays in the cells.
            if (e.PropertyName!="Identifiant"&&e.PropertyName!="=>") {
                FrameworkElementFactory image = new FrameworkElementFactory(typeof(Image));
                image.SetBinding(Image.SourceProperty, new Binding(e.PropertyName));

                e.Column=new DataGridTemplateColumn {
                    CellTemplate=new DataTemplate() { VisualTree=image },
                    Header=e.PropertyName
                };
            }
        }
        private void TabAlarmPagets_Loaded(object sender, RoutedEventArgs e) { LoadProcessesAlarmLevels(); }

        private void LoadProcessesAlarmLevels() {
            var alarmes = LoadAlarmLevelsPerUser();
            PopulateGrid(alarmes);
        }

        private IEnumerable<AlarmeViewModel> LoadAlarmLevelsPerUser() {
            IEnumerable<AlarmeViewModel> alarmes=new List<AlarmeViewModel> {
                new AlarmeViewModel {
                    Identifiant="BOUDA25",
                    Entetes=new List<string> { "100", "110", "130", "150", "205", "220" },
                    Niveaux=new List<short> { 0, 1, 2, 3, 4, 2 }
                },
                new AlarmeViewModel {
                    Identifiant="MARWI34",
                    Entetes=new List<string> { "100", "110", "130", "150", "205", "220" },
                    Niveaux=new List<short> { 4, 3, 2, 1, 0, 1 }
                }
            };

            return alarmes;
        }

        public void PopulateGrid(IEnumerable<AlarmeViewModel> alarmes) {
            var aAfficher = GenerateColumns();
            GenerateRows();
            RefreshDisplay();

            DataTable GenerateColumns() {
                var table = new DataTable();
                table.Columns.Add("Identifiant");
                table.Columns.Add("=>");
                
                alarmes.ToList()
                    .ForEach(a => a.Entetes.ToList()
                        .ForEach(e => { 
                            if (!table.Columns.Contains(name:e)) 
                                table.Columns.Add(columnName: e, type: typeof(string)); 
                        }));

                return table;
            }

            void GenerateRows() {
                var fichiersImages = new string[] {
                    "images/circle_grey.ico",
                    "images/circle_orange.ico",
                    "images/circle_yellow.ico",
                    "images/circle_red.ico",
                    "images/circle_green.ico",
                };

                alarmes.ToList().ForEach(a => {
                    var row = aAfficher.NewRow();
                    row[0]=a.Identifiant;

                    for (int i = 0; i<a.Niveaux.Count; row[a.Entetes[i]]=fichiersImages[a.Niveaux[i]], i++) ;

                    aAfficher.Rows.Add(row);
                });
            }

            void RefreshDisplay() {
                dgAlarmesTraitements.BeginInit();
                dgAlarmesTraitements.ItemsSource=aAfficher.DefaultView;
                dgAlarmesTraitements.EndInit();
            }
        }
    }

    public class AlarmeViewModel {
        public string Identifiant { get; set; }
        public IList<string> Entetes { get; set; }
        public IList<short> Niveaux { get; set; }
    }
}
