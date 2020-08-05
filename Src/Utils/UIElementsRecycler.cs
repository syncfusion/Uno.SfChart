using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using System.Threading.Tasks;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Contains utility methods to add and remove elements inside a panel.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial class UIElementsRecycler<T> : IEnumerable<T> where T : FrameworkElement
    {
        #region fields

        internal List<T> generatedElements {get; set;}

        Panel panel;

        private Dictionary<DependencyProperty, Binding> bindingsProvider;

        #endregion

        #region properties

        /// <summary>
        /// Gets the panel
        /// </summary>
        public Panel Panel
        {
            get { return panel; }
        }

        /// <summary>
        /// Gets the value of CLR property.
        /// </summary>
        public int Count
        {
            get
            {
                return generatedElements.Count;
            }
        }

        /// <summary>
        /// Gets the binding objects to be attached with the generated FrameworkElement.
        /// </summary>
        public Dictionary<DependencyProperty, Binding> BindingProvider
        {
            get
            {
                return bindingsProvider;
            }
        }

        #endregion

        #region ctor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="panel"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public UIElementsRecycler(Panel panel)
        {
            generatedElements = new List<T>();
            bindingsProvider = new Dictionary<DependencyProperty, Binding>();
            this.panel = panel;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public UIElementsRecycler()
        {
            generatedElements = new List<T>();
            bindingsProvider = new Dictionary<DependencyProperty, Binding>();
        }

        #endregion

        #region methods

        /// <summary>
        /// Generates or recycles the elements
        /// </summary>
        /// <param name="count">Number of elements to be generated</param>
        public void GenerateElements(int count)
        {
            T element;
            if (count > generatedElements.Count)
            {
                count = count - generatedElements.Count;
                for (int i = 0; i < count; i++)
                {
                    element = Activator.CreateInstance<T>();

                    foreach (KeyValuePair<DependencyProperty, Binding> bindings in bindingsProvider)
                    {
                        element.SetBinding(bindings.Key, bindings.Value);
                    }

                    generatedElements.Add(element);
                    if (panel != null)
                        panel.Children.Add(element);
                }
            }
            else if (count < generatedElements.Count)
            {
                count = generatedElements.Count - count;

                for (int i = 0; i < count; i++)
                {
                    element = generatedElements.ElementAt(0);
                    generatedElements.Remove(element);
                    if (panel != null && this.panel.Children.Contains(element))
                    {
                        this.panel.Children.Remove(element);
                    }
                }
            }
        }

        /// <summary>
        /// Generates or recycles the elements of the specified type.
        /// Please not the type must be inherited from the FrameworkElement.
        /// </summary>
        /// <param name="count">Number of elements to be generated</param>
        internal void GenerateElementsOfType(int count, Type type)
        {
            T element;
            if (count > generatedElements.Count)
            {
                count = count - generatedElements.Count;
                for (int i = 0; i < count; i++)
                {
                    element = Activator.CreateInstance(type) as T;

                    foreach (KeyValuePair<DependencyProperty, Binding> bindings in bindingsProvider)
                    {
                        element.SetBinding(bindings.Key, bindings.Value);
                    }

                    generatedElements.Add(element);
                    if (panel != null)
                        panel.Children.Add(element);
                }
            }
            else if (count < generatedElements.Count)
            {
                count = generatedElements.Count - count;

                for (int i = 0; i < count; i++)
                {
                    element = generatedElements.ElementAt(0);
                    generatedElements.Remove(element);
                    if (panel != null && this.panel.Children.Contains(element))
                    {
                        this.panel.Children.Remove(element);
                    }
                }
            }
        }

        /// <summary>
        /// Method used to add a element in the panel.
        /// </summary>
        /// <param name="element"></param>

        public void Add(T element)
        {
            foreach (KeyValuePair<DependencyProperty, Binding> bindings in bindingsProvider)
            {
                element.SetBinding(bindings.Key, bindings.Value);
            }

            if (panel != null && !this.panel.Children.Contains(element))
            {
                generatedElements.Add(element);
                panel.Children.Add(element);
            }
        }

        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public int IndexOf(T element)
        {
            return generatedElements.IndexOf(element);
        }

        /// <summary>
        /// Method used to remove the element from the panel.
        /// </summary>
        /// <param name="element"></param>
       
        public void Remove(T element)
        {
            if (panel !=null && this.panel.Children.Contains(element))
            {
                generatedElements.Remove(element);
                panel.Children.Remove(element);
            }
        }

        /// <summary>
        /// Creates a new instance of the specified type
        /// </summary>
        /// <returns></returns>
       
        public T CreateNewInstance()
        {
            T element = Activator.CreateInstance<T>();

            foreach (KeyValuePair<DependencyProperty, Binding> bindings in bindingsProvider)
            {
                element.SetBinding(bindings.Key, bindings.Value);
            }

            generatedElements.Add(element);
            if (panel != null)
                panel.Children.Add(element);

            return element;
        }

        /// <summary>
        /// Removes the particular binding from the generated elements
        /// </summary>
        /// <param name="property"></param>
       
        public void RemoveBinding(DependencyProperty property)
        {
            BindingProvider.Remove(property);
            foreach (T element in generatedElements)
            {
                element.SetBinding(property, null);
            }
        }

        /// <summary>
        /// Clears the generated elements
        /// </summary>
       
        public void Clear()
        {
            if (panel != null)
            {
                foreach (T element in generatedElements)
                {

                    if (this.panel.Children.Contains(element))
                    {
                        this.panel.Children.Remove(element);
                    }
                }
            }

            generatedElements.Clear();
        }

        /// <summary>
        /// Return the panel's child at the corresponding index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
      
        public T this[int index]
        {
            get
            {
                return generatedElements.Count > index ? generatedElements[index] : null;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return this.generatedElements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.generatedElements.GetEnumerator();
        }

        #endregion
    }
}
