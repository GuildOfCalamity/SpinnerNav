using System;
using System.Windows;

namespace SpinnerNav
{
    /// <summary>
    /// Our base attached property to replace the vanilla WPF attached property
    /// </summary>
    /// <typeparam name="Parent">The parent class to be the attached property</typeparam>
    /// <typeparam name="Property">The type of the attached property</typeparam>
    //public abstract class BaseAttachedProperty<Parent, Property> where Parent : BaseAttachedProperty<Parent, Property>, new() //parent has to be our type and new-able
    public abstract class BaseAttachedProperty<Parent, Property> where Parent : new() //parent has to be new-able (XAML does not like generics so we've modified this)
    {
        /// <summary>
        /// A singleton instance of our parent class
        /// </summary>
        public static Parent Instance { get; private set; } = new Parent();

        #region [Events/Properties]
        /// <summary>
        /// Fires when the value changes
        /// </summary>
        public event Action<DependencyObject, DependencyPropertyChangedEventArgs> ValueChanged = (sender, e) => { };

        /// <summary>
        /// Fires when the value changes, even if the value is the same
        /// </summary>
        public event Action<DependencyObject, object> ValueUpdated = (sender, value) => { };

        /// <summary>
        /// Our value property to operate on.
        /// </summary>
        //public static readonly DependencyProperty ValueProperty = DependencyProperty.RegisterAttached("Value", typeof(Property), typeof(BaseAttachedProperty<Parent, Property>), new PropertyMetadata(new PropertyChangedCallback(OnValuePropertyChanged)));
        public static readonly DependencyProperty ValueProperty = DependencyProperty.RegisterAttached(
            "Value",
            typeof(Property),
            typeof(BaseAttachedProperty<Parent, Property>),
            new UIPropertyMetadata(default(Property),
                new PropertyChangedCallback(OnValuePropertyChanged),
                new CoerceValueCallback(OnValuePropertyUpdated)));

        /// <summary>
        /// Our callback event when the <see cref="ValueProperty"/> is changed, even if it's the same value.
        /// </summary>
        /// <param name="d">The UI element that changed</param>
        /// <param name="e">Arguments for the event</param>
        private static object OnValuePropertyUpdated(DependencyObject d, object value)
        {
            //Call the parent function
            (Instance as BaseAttachedProperty<Parent, Property>)?.OnValueUpdated(d, value); //(XAML does not like generics so we've modified this)

            //Call event listeners
            (Instance as BaseAttachedProperty<Parent, Property>)?.ValueUpdated(d, value); //(XAML does not like generics so we've modified this)

            //Return the value
            return value;
        }

        /// <summary>
        /// Our callback event when the <see cref="ValueProperty"/> is changed.
        /// </summary>
        /// <param name="d">The UI element that changed</param>
        /// <param name="e">Arguments for the event</param>
        private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //Call the parent function
            (Instance as BaseAttachedProperty<Parent, Property>)?.OnValueChanged(d, e); //(XAML does not like generics so we've modified this)

            //Call event listeners
            (Instance as BaseAttachedProperty<Parent, Property>)?.ValueChanged(d, e); //(XAML does not like generics so we've modified this)
        }

        /// <summary>
        /// Gets the attached property
        /// </summary>
        /// <param name="d">The element to get the property from</param>
        /// <returns>The property</returns>
        public static Property GetValue(DependencyObject d) => (Property)d.GetValue(ValueProperty);

        /// <summary>
        /// Sets the attached property
        /// </summary>
        /// <param name="d">The element to set the property to</param>
        public static void SetValue(DependencyObject d, Property value) => d.SetValue(ValueProperty, value);
        #endregion

        #region [Event Methods]

        /// <summary>
        /// The method that is called when any attached property of this type is changed.
        /// </summary>
        /// <param name="sender">The UI element that this property is changed for.</param>
        /// <param name="e">The arguments for the event</param>
        public virtual void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            // We will override this in our other classes.
        }

        /// <summary>
        /// The method that is called when any attached property of this type is changed, even if the value is the same
        /// </summary>
        /// <param name="sender">The UI element that this property is changed for.</param>
        /// <param name="e">The arguments for the event</param>
        public virtual void OnValueUpdated(DependencyObject sender, object value)
        {
            // We will override this in our other classes.
        }

        #endregion

        /// <summary>
        /// Utility method for identification.
        /// </summary>
        /// <returns>type of parent as a string</returns>
        public string WhatsMyType()
        {
            return typeof(Parent).ToString();
        }
    }
}
