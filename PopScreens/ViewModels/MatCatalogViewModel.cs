using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using PythoPlus;
using Microsoft.Maui.Controls;

namespace PythoPlus.PopScreens
{
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
            ItemTappedCommand = new Command<Material>(OnItemTapped);
            LoadMaterials();
        }

        private void LoadMaterials()
        {
            for (int i = 1; i <= 3; i++)
            {
                var structure = new List<XmlFields>
                {
                    new XmlFields("MaterialNumber", string.Empty),
                    new XmlFields("MaterialName", string.Empty),
                    new XmlFields("MaterialDescription", string.Empty)
                };
                var service = new XmlFileService(structure, $"PythonMaterial{i}", "Context.xml");
                var fields = service.ReadXml();

                string materialName = fields.FirstOrDefault(f => f.Name == "MaterialName")?.Value;
                string materialDescription = fields.FirstOrDefault(f => f.Name == "MaterialDescription")?.Value;

                Materials.Add(new Material
                {
                    MaterialNumber = i,
                    MaterialName = materialName,
                    MaterialDescription = materialDescription
                });
            }
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

    public class Material
    {
        public int MaterialNumber { get; set; }
        public string MaterialName { get; set; }
        public string MaterialDescription { get; set; }
    }
}
