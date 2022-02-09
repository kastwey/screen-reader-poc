using NonInvasiveKeyboardHookLibrary;

using System;
using System.Diagnostics;
using System.Windows.Automation;
using System.Windows.Forms;
using System.Speech.Synthesis;
namespace ScreenReaderPOC
{
	public partial class Form1 : Form
	{

		private readonly KeyboardHookManager _hookManager = new KeyboardHookManager();
		private readonly SpeechSynthesizer _synthesizer = new SpeechSynthesizer();
		private readonly AutomationFocusChangedEventHandler _focusHandler = null;

		public Form1()
		{
			InitializeComponent();
			_synthesizer.SelectVoice(_synthesizer.GetInstalledVoices()[0].VoiceInfo.Name);
			_focusHandler = new AutomationFocusChangedEventHandler(OnFocusChange);
		}

		private void OnFocusChange(object sender, AutomationFocusChangedEventArgs e)
		{
			var element = sender as AutomationElement;
			if (element != null)
			{
				SayElementName(element);
			}
		}

		private void SayElementName(AutomationElement element)
		{
			var namePropertyValue = element.GetCurrentPropertyValue(AutomationElement.NameProperty) as string;
			if (namePropertyValue != null)
			{
				Debug.WriteLine("Name: " + namePropertyValue);
				_synthesizer.Speak(namePropertyValue);
			}
		}


		private void Form1_Load(object sender, EventArgs e)
		{
			_hookManager.RegisterHotkey(NonInvasiveKeyboardHookLibrary.ModifierKeys.Shift | NonInvasiveKeyboardHookLibrary.ModifierKeys.Control, 0x7B, () =>
			{
				Debug.WriteLine("Pulosado!");
				var focusedElement = AutomationElement.FocusedElement;
				SayElementName(focusedElement);
			});
			_hookManager.Start();
			Automation.AddAutomationFocusChangedEventHandler(_focusHandler);
		}

		private void Form1_FormClosed(object sender, FormClosedEventArgs e)
		{
			_hookManager.Stop();
			if (_focusHandler != null)
			{
				Automation.RemoveAutomationFocusChangedEventHandler(_focusHandler);
			}
		}

	}
}
