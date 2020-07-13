using Microsoft.EntityFrameworkCore;
using MVVM_SQlite_LocalDB.Commands;
using MVVM_SQlite_LocalDB.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MVVM_SQlite_LocalDB.ViewModels
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        //DbContext
        private MachineContext db;

        public MainViewModel()
        {
            db = new MachineContext();

            //Creates database if non created and perform migrations
            db.Database.Migrate();
            Machines = new ObservableCollection<Machine>();
            ResetFields();
        }

        //Propert for storing Machine object for modifications
        private Machine selectedMachine;
        public Machine SelectedMachine
        {
            get { return selectedMachine; }
            set { selectedMachine = value; NotifyPropertyChanged(); CanEditId = false; NotifyPropertyChanged("CanUpdate"); }
        }

        //Property to prevent editing Id during update operation
        private bool canEditId;
        public bool CanEditId
        {
            get { return !canEditId; }
            set { canEditId = value; NotifyPropertyChanged(); }
        }

        //Text for displaying errors and warnings
        private string infoText;
        public string InfoText
        {
            get { return infoText; }
            set { infoText = value; NotifyPropertyChanged(); }
        }

        public ICollection<Machine> Machines
        {
            get;
            private set;
        }

        private void GetMachineList()
        {
            Machines.Clear();
            foreach (var machine in db.Machines.OrderBy(p => p.Id).ToArray())
                Machines.Add(machine);
        }

        //Property to check if operations are possible with the current state of Selected Machine.
        public bool CanModify
        {
            get
            {
                if (SelectedMachine == null) return false;
                return !String.IsNullOrWhiteSpace(selectedMachine.Name) && selectedMachine.Id >= 0;
            }
        }



        public ICommand UpdateCommand
        {
            get
            {
                return new ActionCommand(p => UpdateMachine(), p => CanModify);
            }
        }

        public ICommand AddCommand
        {
            get
            {
                return new ActionCommand(p => AddMachine(), p => CanModify);
            }
        }

        public ICommand DeleteCommand
        {
            get
            {
                return new ActionCommand(p => DeleteMachine(), p => CanModify);
            }
        }

        public ICommand ResetCommand
        {
            get
            {
                return new ActionCommand(p => ResetFields());
            }
        }

        private void UpdateMachine()
        {
            try
            {
                var toUpdate = db.Machines.Where(m => m.Id == SelectedMachine.Id).Single();
                if (toUpdate == null)
                {
                    InfoText = "Entry not found!";
                    return;
                }
                toUpdate.Name = SelectedMachine.Name;
                toUpdate.Description = SelectedMachine.Description;
                db.SaveChanges();
                ResetFields();
            }
            catch (Exception)
            {
                InfoText = "Entry not found!";
                return;
            }

        }

        private void AddMachine()
        {
            try
            {
                if (db.Machines.Find(selectedMachine.Id) != null)
                {
                    InfoText = "Id exists, try another Id!";
                    return;
                }
                db.Machines.Add(SelectedMachine);
                db.SaveChanges();
                ResetFields();
            }
            catch (Exception)
            {

                InfoText = "Add failed!";
                return;
            }
        }

        private void DeleteMachine()
        {
            var toDelete = db.Machines.Where(m => m.Id == SelectedMachine.Id).Single();
            if (toDelete == null)
            {
                throw new NotImplementedException();
            }
            db.Machines.Remove(toDelete);
            db.SaveChanges();
            ResetFields();
        }

        private void ResetFields()
        {
            GetMachineList();
            this.SelectedMachine = new Machine();
            CanEditId = true;
            InfoText = "";
        }


        /// <summary>
        /// Event handler for PropertChanged, informs MainWIndow of property change.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
