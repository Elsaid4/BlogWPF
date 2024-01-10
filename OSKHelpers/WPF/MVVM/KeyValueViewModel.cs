using CommunityToolkit.Mvvm.ComponentModel;

namespace OSKHelpers.WPF.MVVM
{
	/// <summary>
	/// Classe di utilità per unità chiave / valore fortemente tipizzate.<br/>
	/// L'utilizzo ideale non è quello di sfruttarle come dizionari (c'è la classe apposita) ma per liste che vadano a popolare
	/// combobox o quant'altro.<br/>
	/// </summary>
	/// <typeparam name="KeyType">Tipo della chiave</typeparam>
	/// <typeparam name="ValueType">Tipo del valore</typeparam>
	public class KeyValueViewModel<KeyType, ValueType> : ObservableObject
	{
		#region Membri

		private KeyType _key;
		private ValueType _value;

		#endregion

		#region Proprietà

		public KeyType Key
		{
			get => _key;
			set => SetProperty(ref _key, value);
		}
		public ValueType Value
		{
			get => _value;
			set => SetProperty(ref _value, value);
		}

		#endregion

		#region Costruttori

		public KeyValueViewModel()
		{
		}

		public KeyValueViewModel(KeyType key, ValueType value)
		{
			Key = key;
			Value = value;
		}

		#endregion
	}
}