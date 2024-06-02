using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using PythoPlus;
using Microsoft.Maui.Controls;

namespace PythoPlus.PopScreens
{
    //
    //                  УСТАРЕВШАЯ ВЕРСИЯ!
    //              Более не используется никак, кроме как бекап.
    //
    public class MatCatalogViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Material> materials;
        public ObservableCollection<Material> Materials
        {
            get => materials;
            set
            {
                materials = value;
                OnPropertyChanged();
            }
        }

        public ICommand ItemTappedCommand { get; private set; }

        public MatCatalogViewModel()
        {
            Materials = new ObservableCollection<Material>();
            LoadMedium();
            ItemTappedCommand = new Command<Material>(OnItemTapped);
        }

        public void LoadMedium()
        {
            LoadMaterialsAsync();
        }

        private async void LoadMaterialsAsync()
        {
            var superStructure = new List<XmlFields>
            {
                new XmlFields("MaterialFirstNaming", string.Empty),
                new XmlFields("MaterialContextNaming", string.Empty),
                new XmlFields("MaterialContentNaming", string.Empty),
                new XmlFields("MaterialCount", string.Empty),
            };
            var superService = new XmlFileService(superStructure);

            var superInfo = await superService.ReadXmlAsync();

            string matFsn = superInfo.FirstOrDefault(f => f.Name == "MaterialFirstNaming")?.Value;
            string matCxn = superInfo.FirstOrDefault(f => f.Name == "MaterialContextNaming")?.Value;
            string matCntForm = superInfo.FirstOrDefault(f => f.Name == "MaterialCount")?.Value;

            int matCnt = int.Parse(matCntForm);

            for (int i = 1; i <= matCnt; i++)
            {
                var structure = new List<XmlFields>
                {
                    new XmlFields("MaterialNumber", string.Empty),
                    new XmlFields("MaterialName", string.Empty),
                    new XmlFields("MaterialDescription", string.Empty)
                };
                var service = new XmlFileService(structure, $"{matFsn}{i}", $"{matCxn}");
                var fields = await service.ReadXmlAsync();

                string materialName = fields.FirstOrDefault(f => f.Name == "MaterialName")?.Value;
                string materialDescription = fields.FirstOrDefault(f => f.Name == "MaterialDescription")?.Value;

                Materials.Add(new Material
                {
                    MaterialNumber = i,
                    MaterialName = materialName,
                    MaterialDescription = materialDescription
                });
            }
            OnPropertyChanged();
            return;
        }

        private void OnItemTapped(Material material)
        {
            // Условный обработчик нажатия, например, навигация к детальной информации о материале
            Console.WriteLine($"Tapped on material number: {material.MaterialNumber}");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
