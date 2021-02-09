﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using XamForms.PickerView;
using XamForms.PickerView.iOS;
using Font = Xamarin.Forms.Font;

[assembly: ExportRenderer(typeof(PickerView), typeof(PickerViewRenderer))]

namespace XamForms.PickerView.iOS
{
    public class PickerViewRenderer : ViewRenderer<PickerView, UIPickerView>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<PickerView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                SetNativeControl(new UIPickerView(RectangleF.Empty));
            }

            if (e.NewElement != null)
            {
                UpdateItemsSource();
                UpdateSelectedIndex();
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == PickerView.ItemsSourceProperty.PropertyName)
            {
                UpdateItemsSource();
            }
            else if (e.PropertyName == PickerView.SelectedIndexProperty.PropertyName)
            {
                UpdateSelectedIndex();
            }
            else if (e.PropertyName == PickerView.FontSizeProperty.PropertyName)
            {
                UpdateItemsSource();
            }
            else if (e.PropertyName == PickerView.FontFamilyProperty.PropertyName)
            {
                UpdateItemsSource();
            }
        }

        private void UpdateItemsSource()
        {
            var font = string.IsNullOrEmpty(Element.FontFamily) ?
                Font.SystemFontOfSize(Element.FontSize) :
                Font.OfSize(Element.FontFamily, Element.FontSize);
            var nativeFont = font.ToUIFont();
            Control.Model = new MyDataModel(Element.ItemsSource, row =>
            {
                Element.SelectedIndex = row;
            }, nativeFont);
        }

        private void UpdateSelectedIndex()
        {
            if (Control.Model == null)
            {
                return;
            }

            if (Element.SelectedIndex < 0 || Element.SelectedIndex >= Control.Model.GetRowsInComponent(Control, 0))
            {
                return;
            }

            Control.Select(Element.SelectedIndex, 0, true);
        }
    }

    internal class MyDataModel : UIPickerViewModel
    {
        private readonly IList<string> _list = new List<string>();
        private readonly Action<int> _selectedHandler;
        private readonly UIFont _nativeFont;

        public MyDataModel(IEnumerable items, Action<int> selectedHandler, UIFont nativeFont)
        {
            _selectedHandler = selectedHandler;
            _nativeFont = nativeFont;

            if (items != null)
            {
                foreach (var item in items)
                {
                    _list.Add(item.ToString());
                }
            }
        }

        public override nint GetComponentCount(UIPickerView pickerView)
        {
            return 1;
        }

        public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
        {
            return _list.Count;
        }

        public override string GetTitle(UIPickerView pickerView, nint row, nint component)
        {
            return _list[(int)row];
        }

        public override UIView GetView(UIPickerView pickerView, nint row, nint component, UIView view)
        {
            UILabel label = new UILabel(pickerView.Bounds)
            {
                Font = _nativeFont,
                Text = _list[(int)row],
                TextAlignment = UITextAlignment.Center
            };
            return label;

            //return base.GetView(pickerView, row, component, view);
        }

        public override void Selected(UIPickerView pickerView, nint row, nint component)
        {
            _selectedHandler?.Invoke((int)row);
        }
    }
}