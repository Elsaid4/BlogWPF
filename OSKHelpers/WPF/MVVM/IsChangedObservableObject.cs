using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace OSKHelpers.WPF.MVVM
{
    public class IsChangedObservableObject : ObservableObject
    {
        #region Membri

        private bool _isChanged;
        private bool _disableIsChangedTracking;
        private bool _disableDependingPropertiesNotification;
        private bool _disableDependingEventsNotification;
        private List<string> _doNotSetIsChangedProperties;
        private Dictionary<string, List<string>> _dependentPropertiesToNotify;
        private Dictionary<string, List<RelayCommand>> _dependentEventsToNotify;

        private bool _doNotSetIsChangedPropertiesIsNull = true;
        private bool _dependentPropertiesToNotifyIsNull = true;
        private bool _dependentEventsToNotifyIsNull     = true;

        #endregion

        #region Proprietà

        public bool IsChanged 
        { 
            get => _isChanged;
            set
            {
                SetProperty(ref _isChanged, value);
                Changed?.Invoke(this, EventArgs.Empty);
                IsChangedValueChanged();
            }
        }

        /// <summary>
        /// Contiene i nomi delle proprietà ignorati nella valutazione di IsChanged.<br/>
        /// </summary>
        public List<string> DoNotSetIsChangedProperties 
        { 
            get => _doNotSetIsChangedProperties;
            protected set
            {
                _doNotSetIsChangedProperties        = value;
                _doNotSetIsChangedPropertiesIsNull  = value == null;
            }
        }

        /// <summary>
        /// Dizionario contenente come chiave il nome della proprietà la cui variazione porterà al lancio 
        /// dell'evento PropertyChanged per le proprietà conenute in Value<br/>
        /// ATTENZIONE: la modifica della proprietà comporterà la modifica di DisableDependentPropertiesNotification<br/>
        /// Nel seguente codice la modifica di Val1 a "Y" comporterà anche gli eventi PropertyChanged per Val2 e Val3
        /// <code>
        /// Val1 = "X";
        /// DependingPropertiesToNotify.Add("Val1", new List&lt;string&gt;() { "Val2", "Val3"});
        /// Val1 = "Y";
        /// </code>
        /// A termine di questa parte di codice saranno eseguiti i seguenti metodi:
        /// <code>
        /// OnPropertyChanged("Val1");
        /// OnPropertyChanged("Val2");
        /// OnPropertyChanged("Val3");
        /// </code>
        /// <strong>IMPORTANTE</strong>: nel caso in cui venga utilizzata questa funzionalità è utile lanciare dopo 
        /// l'inizializzazione la verifica della validità di quanto inserito utilizzando il metodo 
        /// <em>CheckCircularNotifications</em>
        /// </summary>
        public Dictionary<string, List<string>> DependentPropertiesToNotify
        {
            get => _dependentPropertiesToNotify;
            protected set
            {
                _dependentPropertiesToNotify            = value;
                _dependentPropertiesToNotifyIsNull      = value == null;
                DisableDependingPropertiesNotification  = value == null;
            }
        }

        /// <summary>
        /// Dizionario contenente come chiave il nome della proprietà la cui variazione porterà all'aggiornamento
        /// di CanExecute per gli eventi presenti in Value.<br/>
        /// ATTENZIONE: la modifica della proprietà comporterà la modifica di DisableDependentEventsNotification<br/>
        /// Nel seguente codice la modifica di Val1 a "Y" comporterà l'aggiornamento di CanExecute per Cmd1 e Cmd2
        /// <code>
        /// Val1 = "X";
        /// DependingPropertiesToNotify.Add("Val1", new List&lt;RelayCommand&gt;() { Cmd1, Cmd2 });
        /// Val1 = "Y";
        /// </code>
        /// A termine di questa parte di codice saranno eseguiti i seguenti metodi:
        /// <code>
        /// OnPropertyChanged("Val1");
        /// Cmd1.NotifyCanExecuteChanged();
        /// Cmd2.NotifyCanExecuteChanged();
        /// </code>
        /// </summary>
        public Dictionary<string, List<RelayCommand>> DependentEventsToNotify 
        { 
            get => _dependentEventsToNotify;
            protected set
            {
                _dependentEventsToNotify            = value;
                _dependentEventsToNotifyIsNull      = value == null;
                DisableDependingEventsNotification  = value == null;
            }
        }

        /// <summary>
        /// Evento elevato quando il valore di IsChanged cambia
        /// </summary>
        public event EventHandler Changed;

        /// <summary>
        /// Disabilita il tracciamento della modifica dei valori delle proprietà
        /// </summary>
        public bool DisableIsChangedTracking 
        { 
            get => _disableIsChangedTracking; 
            set => _disableIsChangedTracking = value; 
        }

        /// <summary>
        /// Disabilita la notifica in caduta della variazione del valore delle proprietà dipendenti
        /// </summary>
        public bool DisableDependingPropertiesNotification 
        { 
            get => _disableDependingPropertiesNotification; 
            set => _disableDependingPropertiesNotification = value; 
        }

        /// <summary>
        /// Disabilita la notifica dell'aggiornamento di CanExecute per i comandi dipendenti
        /// </summary>
        public bool DisableDependingEventsNotification 
        { 
            get => _disableDependingEventsNotification; 
            set => _disableDependingEventsNotification = value; 
        }

        #endregion

        #region Costruttori

        public IsChangedObservableObject()
        {
            DoNotSetIsChangedProperties = null;
            DependentPropertiesToNotify = null;
            DependentEventsToNotify     = null;
            PropertyChanged             += OnPropertyValueIsChanged;
            IsChanged                   = false;
            DisableIsChangedTracking    = false;
        }

        #endregion

        #region Metodi

        private void OnPropertyValueIsChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!_disableIsChangedTracking && !_isChanged && e.PropertyName != nameof(IsChanged) && (_doNotSetIsChangedPropertiesIsNull || !DoNotSetIsChangedProperties.Contains(e.PropertyName)))
            {
                IsChanged = true;
            }
            if  (!_disableDependingPropertiesNotification && !_dependentPropertiesToNotifyIsNull && DependentPropertiesToNotify.ContainsKey(e.PropertyName))
            {
                foreach (var dependingProperty in DependentPropertiesToNotify[e.PropertyName])
                {
                    OnPropertyChanged(dependingProperty);
                }
            }
            if (!_disableDependingEventsNotification && !_dependentEventsToNotifyIsNull && DependentEventsToNotify.ContainsKey(e.PropertyName))
            {
                foreach (var cmd in DependentEventsToNotify[e.PropertyName])
                {
                    cmd.NotifyCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// Metodo di cui eseguire l'overload se si desiderano effettaure operazioni personalizzate al cambiamento di IsChanged
        /// </summary>
        protected virtual void IsChangedValueChanged()
        { }

        /// <summary>
        /// Verifica che non ci siano riferimenti circolari nelle notifiche di aggiornamento delle proprietà.<br/>
        /// Nel caso in cui venga rilevato un riferimento circolare questo porterà alla generazione di un'eccezione di tipo
        /// InvalidOperationException con indicate la proprietà verificata ed il percorso che genera il riferimento circolare.<br/>
        /// L'elaborazione terminerà al primo errore rilevato.
        /// </summary>
        /// <exception cref="InvalidOperationException">Eccezione lanciata nel caso vengano rilevati riferimenti circolari.</exception>
        public void CheckCircularNotifications()
        {
            foreach (var property in DependentPropertiesToNotify)
            {
                var propertyList = new List<string>();
                if (!CheckDependingProperty(property.Key, propertyList))
                {
                    throw new InvalidOperationException($"Errore di riferimento ciclico nelle proprietà dipendenti: {string.Join(" > ", propertyList)}");
                }
            }
        }

        private bool CheckDependingProperty(string property, List<string> propertyList)
        {
            bool ok = true;
            Console.WriteLine($"{string.Join(" > ", propertyList)}");
            if (DependentPropertiesToNotify.ContainsKey(property) && DependentPropertiesToNotify[property].Any())
            {
                propertyList.Add(property);
                foreach (var p in DependentPropertiesToNotify[property])
                {
                    if (ok)
                    {
                        ok = !propertyList.Contains(p) && CheckDependingProperty(p, propertyList);
                    }
                }
                if (ok)
                {
                    propertyList.RemoveAt(propertyList.Count - 1);
                }
            }
            return ok;
        }

        #endregion
    }
}
