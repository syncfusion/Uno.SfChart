using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// A collection class which holds ChartBehaviors.
    /// </summary>
    
    public partial class ChartBehaviorsCollection : ObservableCollection<ChartBehavior>
    {
        internal SfChart Area;

        /// <summary>
        /// Called when instance created for ChartBehaviourCollection
        /// </summary>
        /// <param name="area"></param>
        public ChartBehaviorsCollection(SfChart area)
        {
            Area = area;
        }

        /// <summary>
        /// Called when instance created for ChartBehaviorsCollection
        /// </summary>
        public ChartBehaviorsCollection()
        {

        }

        /// <summary>
        /// Inserts an item into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param><param name="item">The object to insert.</param>
        protected override void InsertItem(int index, ChartBehavior item)
        {
            item.ChartArea = Area;
            item.AdorningCanvas = Area.GetAdorningCanvas();
            if (item.AdorningCanvas != null)
                item.InternalAttachElements();
            base.InsertItem(index, item);
        }

        /// <summary>
        /// Removes the item at the specified index of the collection.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        protected override void RemoveItem(int index)
        {
            var item = this.Items[index];            
            item.DetachElements();
            item.ChartArea = Area;
            base.RemoveItem(index);
        }
        protected override void ClearItems()
        {
            foreach (ChartBehavior behavior in Items)
            {
                behavior.DetachElements();
                behavior.ChartArea = Area;
            }

            base.ClearItems();
        }
    }

    /// <summary>
    /// Represents a collection of <see cref="ChartAxisLabel"/>.
    /// </summary>
   
    public partial class ChartAxisLabelCollection : ObservableCollection<ChartAxisLabel>
    {
        /// <summary>
        /// Inserts an item into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param><param name="item">The object to insert.</param>
        protected override void InsertItem(int index, ChartAxisLabel item)
        {
            base.InsertItem(index, item);
        }

        /// <summary>
        /// ChartAxisLabelsCollection Clear Items
        /// </summary>    
        /// <seealso>
        ///     <cref>ChartAxisLabelsCollection</cref>
        /// </seealso>
        protected override void ClearItems()
        {
            base.ClearItems();
        }
    }
}
