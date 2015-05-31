﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.Windows.Forms;

namespace PlaylistBox
{
    internal class ChatListItemConverter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
            //MessageBox.Show("CanConvertTo:" + (destinationType == typeof(InstanceDescriptor)));
            return destinationType == typeof(InstanceDescriptor) ||
                base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture,
            object value, Type destinationType) {
            if (destinationType == null)
                throw new ArgumentNullException("DestinationType cannot be null");
            //MessageBox.Show("Convertto OK");
            if (destinationType == typeof(InstanceDescriptor) && (value is PlayList)) {
                ConstructorInfo constructor = null;
                PlayList item = (PlayList)value;
                SongItem[] subItems = null;
                //MessageBox.Show("Convertto Start Item:" + item.Text);
                //MessageBox.Show("Item.SubItems.Count:" + item.SubItems.Count);
                if (item.SubItems.Count > 0) {
                    subItems = new SongItem[item.SubItems.Count];
                    item.SubItems.CopyTo(subItems, 0);
                }
                //MessageBox.Show("Item.SubItems.Count:" + item.SubItems.Count);
                if (item.Text != null && subItems != null)
                    constructor = typeof(PlayList).GetConstructor(new Type[] { typeof(string), typeof(SongItem[]) });
                //MessageBox.Show("Constructor(Text,item[]):" + (constructor != null));
                if (constructor != null)
                    return new InstanceDescriptor(constructor, new object[] { item.Text, subItems }, false);
                
                if (subItems != null)
                    constructor = typeof(PlayList).GetConstructor(new Type[] { typeof(SongItem[]) });
                if (constructor != null)
                    return new InstanceDescriptor(constructor, new object[] { subItems }, false);
                if (item.Text != null) {
                    //MessageBox.Show("StartGetConstructor(text)");
                    constructor = typeof(PlayList).GetConstructor(new Type[] { typeof(string), typeof(bool) });
                }
                //MessageBox.Show("Constructor(Text):" + (constructor != null));
                if (constructor != null) {
                    //System.Windows.Forms.MessageBox.Show("text OK");
                    return new InstanceDescriptor(constructor, new object[] { item.Text, item.IsOpen });
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
