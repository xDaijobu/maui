using System;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Controls.Compatibility.Platform.Tizen.Native;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Graphics;
using EEntry = ElmSharp.Entry;
using NIEntry = Microsoft.Maui.Controls.Compatibility.Platform.Tizen.Native.IEntry;

namespace Microsoft.Maui.Controls.Compatibility.Platform.Tizen
{
	[System.Obsolete(Compatibility.Hosting.MauiAppBuilderExtensions.UseMapperInstead)]
	public class EditorRenderer : ViewRenderer<Editor, EEntry>
	{
		public EditorRenderer()
		{
			RegisterPropertyHandler(Editor.TextProperty, UpdateText);
			RegisterPropertyHandler(Editor.TextColorProperty, UpdateTextColor);
			RegisterPropertyHandler(Editor.FontSizeProperty, UpdateFontSize);
			RegisterPropertyHandler(Editor.FontFamilyProperty, UpdateFontFamily);
			RegisterPropertyHandler(Editor.FontAttributesProperty, UpdateFontAttributes);
			RegisterPropertyHandler(InputView.KeyboardProperty, UpdateKeyboard);
			RegisterPropertyHandler(InputView.MaxLengthProperty, UpdateMaxLength);
			RegisterPropertyHandler(InputView.IsSpellCheckEnabledProperty, UpdateIsSpellCheckEnabled);
			RegisterPropertyHandler(Editor.PlaceholderProperty, UpdatePlaceholder);
			RegisterPropertyHandler(Editor.PlaceholderColorProperty, UpdatePlaceholderColor);
			RegisterPropertyHandler(InputView.IsReadOnlyProperty, UpdateIsReadOnly);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
		{
			if (Control == null)
			{
				var entry = CreateNativeControl();
				entry.Focused += OnEntryFocused;
				entry.Unfocused += OnEntryUnfocused;
				entry.Unfocused += OnCompleted;
				entry.PrependMarkUpFilter(MaxLengthFilter);
				if (entry is NIEntry ie)
				{
					ie.TextChanged += OnTextChanged;
					ie.EntryLayoutFocused += OnFocused;
					ie.EntryLayoutUnfocused += OnUnfocused;
				}
				SetNativeControl(entry);
			}
			base.OnElementChanged(e);
		}

		protected virtual EEntry CreateNativeControl()
		{
			// Multiline EditField style is only available on Mobile and TV profile
			var entry = DeviceInfo.Idiom == DeviceIdiom.Phone || DeviceInfo.Idiom == DeviceIdiom.TV ? new EditfieldEntry(Forms.NativeParent, EditFieldEntryLayout.Styles.MulitLine) : new Native.Entry(Forms.NativeParent)
			{
				IsSingleLine = false,
			};

			return entry;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (null != Control)
				{
					Control.BackButtonPressed -= OnCompleted;
					Control.Unfocused -= OnEntryUnfocused;
					Control.Focused -= OnEntryFocused;
					if (Control is NIEntry ie)
					{
						ie.TextChanged -= OnTextChanged;
						ie.EntryLayoutFocused -= OnFocused;
						ie.EntryLayoutUnfocused -= OnUnfocused;
					}
				}
			}
			base.Dispose(disposing);
		}

		protected override Size MinimumSize()
		{
			return (Control as Native.IMeasurable).Measure(Control.MinimumWidth, Control.MinimumHeight).ToDP();
		}

		protected virtual void UpdateTextColor()
		{
			if (Control is NIEntry ie)
			{
				ie.TextColor = Element.TextColor.ToPlatformEFL();
			}
		}

		void OnTextChanged(object sender, EventArgs e)
		{
			Element.SetValueFromRenderer(Editor.TextProperty, Control.Text);
		}

		bool _isSendComplate = false;

		void OnEntryFocused(object sender, EventArgs e)
		{
			// BackButtonPressed is only passed to the object that is at the highest Z-Order, and it does not propagate to lower objects.
			// If you want to make Editor input completed by using BackButtonPressed, you should subscribe BackButtonPressed event only when Editor gets focused.
			Control.BackButtonPressed += OnCompleted;
			_isSendComplate = false;
		}

		void OnEntryUnfocused(object sender, EventArgs e)
		{
			// BackButtonPressed is only passed to the object that is at the highest Z-Order, and it does not propagate to lower objects.
			// When the object is unfocesed BackButtonPressed event has to be released to stop using it.
			Control.BackButtonPressed -= OnCompleted;
			if (!_isSendComplate)
				Element.SendCompleted();
		}

		void OnCompleted(object sender, EventArgs e)
		{
			_isSendComplate = true;
			Control.SetFocus(false);
			Element.SendCompleted();
		}

		void UpdateText()
		{
			Control.Text = Element.Text ?? "";
			if (!Control.IsFocused)
			{
				Control.MoveCursorEnd();
			}
		}

		void UpdateFontSize()
		{
			if (Control is NIEntry ie)
			{
				ie.FontSize = Element.FontSize;
			}
		}

		void UpdateFontFamily()
		{
			if (Control is NIEntry ie)
			{
				ie.FontFamily = Element.FontFamily.ToNativeFontFamily(Element.RequireFontManager());
			}
		}

		void UpdateFontAttributes()
		{
			if (Control is NIEntry ie)
			{
				ie.FontAttributes = Element.FontAttributes;
			}
		}

		void UpdateKeyboard(bool initialize)
		{
			if (initialize && Element.Keyboard == Keyboard.Default)
				return;

			if (Control is NIEntry ie)
			{
				ie.UpdateKeyboard(Element.Keyboard, Element.IsSpellCheckEnabled, true);
			}
		}

		void UpdateIsSpellCheckEnabled()
		{
			Control.InputHint = Element.Keyboard.ToInputHints(Element.IsSpellCheckEnabled, true);
		}

		void UpdateMaxLength()
		{
			if (Control.Text.Length > Element.MaxLength)
				Control.Text = Control.Text.Substring(0, Element.MaxLength);
		}

		void UpdatePlaceholder()
		{
			if (Control is NIEntry ie)
			{
				ie.Placeholder = Element.Placeholder;
			}
		}

		void UpdatePlaceholderColor()
		{
			if (Control is NIEntry ie)
			{
				ie.PlaceholderColor = Element.PlaceholderColor.ToPlatformEFL();
			}
		}

		string MaxLengthFilter(ElmSharp.Entry entry, string s)
		{
			if (entry.Text.Length < Element.MaxLength)
				return s;

			return null;
		}

		void UpdateIsReadOnly()
		{
			Control.IsEditable = !Element.IsReadOnly;
		}
	}
}