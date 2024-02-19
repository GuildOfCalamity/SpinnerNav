using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace SpinnerNav
{
    /// <summary>
    /// Base class to run any animation method when a boolean is set to true
    /// and a reverse animation when set to false.
    /// </summary>
    /// <typeparam name="Parent"></typeparam>
    public abstract class AnimateBaseProperty<Parent> : BaseAttachedProperty<Parent, bool>
                                         where Parent : BaseAttachedProperty<Parent, bool>, new()
    {
        /// <summary>
        /// True if this is the very first time the value has been updated.
        /// Used to make sure we run the logic at least once during first load.
        /// </summary>
        protected Dictionary<DependencyObject, bool> mAlreadyLoaded = new Dictionary<DependencyObject, bool>();

        /// <summary>
        /// The most recent value used if we get a value changed before we do the first load.
        /// </summary>
        protected Dictionary<DependencyObject, bool> mFirstLoadValue = new Dictionary<DependencyObject, bool>();

        /// <summary>
        /// Indicate if this is the first time this property has been loaded.
        /// </summary>
        public bool FirstLoad { get; set; } = true;

        /// <summary>
        /// This is used to make sure we run the logic at least once during first load.
        /// </summary>
        protected bool mFirstFire = true;

        public override void OnValueUpdated(DependencyObject sender, object value)
        {
            // Are we a FrameworkElement?
            // If we are then copy it into the "element" variable for local use.
            if (!(sender is FrameworkElement element))
                return;


            // Don't fire if the value doesn't change
            if ((bool)sender.GetValue(ValueProperty) == (bool)value && mAlreadyLoaded.ContainsKey(sender))
                return;

            // On first load...
            if (!mAlreadyLoaded.ContainsKey(sender))
            {
                // Flag that we are in first load but have not finished it
                mAlreadyLoaded[sender] = false;

                // Start off hidden before we decide how to animate
                // if we are to be animated out initially
                if ((bool)value) { element.Visibility = Visibility.Hidden; }

                // Create a single self-unhookable event 
                // for the elements Loaded event
                RoutedEventHandler onLoaded = null;
                onLoaded = async (ss, ee) =>
                {
                    // Unhook ourselves
                    element.Loaded -= onLoaded;

                    // Slight delay after load is needed for some elements to get laid out
                    // and their width/heights correctly calculated
                    //await Task.Delay(5);

                    // Do desired animation
                    DoAnimation(element, mFirstLoadValue.ContainsKey(sender) ? mFirstLoadValue[sender] : (bool)value, true);

                    // Flag that we have finished first load
                    mAlreadyLoaded[sender] = true;

                    // This is only for blur now
                    FirstLoad = false;
                };

                // Hook into the Loaded event of the element
                element.Loaded += onLoaded;
            }
            // If we have started a first load but not fired the animation yet, update the property
            else if (mAlreadyLoaded[sender] == false)
                mFirstLoadValue[sender] = (bool)value;
            else
                // Do desired animation
                DoAnimation(element, (bool)value, false);

            /*

            // Don't fire is value is the same.
            if ((bool)sender.GetValue(ValueProperty) == (bool)value && !mFirstFire)
                return;

            // No longer first fire
            mFirstFire = false;

            // On first time, create an event that will fire once and then unhook itself.
            if (FirstLoad)
            {
                //if (!(bool)value)
                //    element.Visibility = Visibility.Hidden;

                RoutedEventHandler onLoaded = null;
                
                // Setup our delegate so we can wait for the
                // element to load before performing our animation.
                onLoaded = async (ss, ee) =>
                {
                    // Un-hook ourselves (make sure this is first before any other code)
                    element.Loaded -= onLoaded;

                    // Add slight delay so that elements have time to get laid
                    // out so their width & height can be calculated correctly.
                    //await Task.Delay(1);

                    // Execute any animation stuff.
                    DoAnimation(element, (bool)value);

                    // Don't do this again
                    FirstLoad = false;
                };

                // Hook into the element
                element.Loaded += onLoaded;

            }
            else // If we're called again then do the animation routine
            {
                // Execute any animation stuff.
                DoAnimation(element, (bool)value);
            }
            */
        }

        /// <summary>
        /// Animation method when the event value changes.
        /// </summary>
        /// <remarks>
        /// We can override this because it's virtual.
        /// </remarks>
        protected virtual void DoAnimation(FrameworkElement element, bool value, bool firstLoad) { }
    }

    /// <summary>
    /// Fades in an image once the source changes.
    /// </summary>
    public class FadeInImageOnLoadProperty : AnimateBaseProperty<FadeInImageOnLoadProperty>
    {
        public override void OnValueUpdated(DependencyObject sender, object value)
        {
            // Only operate if element is image.
            if (!(sender is Image image))
                return;

            // Always use TargetUpdated instead of SourceUpdated.
            if ((bool)value)
                image.TargetUpdated += Image_TargetUpdatedAsync;
            else
                image.TargetUpdated -= Image_TargetUpdatedAsync;
        }

        /// <summary>
        /// Image control event.
        /// </summary>
        private async void Image_TargetUpdatedAsync(object? sender, System.Windows.Data.DataTransferEventArgs e)
        {
            if (sender != null && sender is Image img)
            {
                // Do our animation
                await img.FadeInAsync(false);
            }
        }
    }

    /// <summary>
    /// Animates a framework element sliding it in from the left on show and sliding out to the left on hide.
    /// </summary>
    public class AnimateSlideInFromLeftProperty : AnimateBaseProperty<AnimateSlideInFromLeftProperty>
    {
        protected override async void DoAnimation(FrameworkElement element, bool value, bool firstLoad)
        {
            if (value) // Animate in
                await element.SlideAndFadeInAsync(AnimationSlideInDirection.Left, firstLoad, firstLoad ? 0 : 0.6f, keepMargin: false);
            else       // Animate out
                await element.SlideAndFadeOutAsync(AnimationSlideInDirection.Left, firstLoad ? 0 : 0.6f, keepMargin: false);
        }
    }

    /// <summary>
    /// Animates a framework element sliding it in from the left on show and sliding out to the left on hide.
    /// </summary>
    public class AnimateSlideInFromLeftKeepMarginProperty : AnimateBaseProperty<AnimateSlideInFromLeftKeepMarginProperty>
    {
        protected override async void DoAnimation(FrameworkElement element, bool value, bool firstLoad)
        {
            if (value) // Animate in
                await element.SlideAndFadeInAsync(AnimationSlideInDirection.Left, firstLoad, firstLoad ? 0 : 0.6f, keepMargin: true);
            else       // Animate out
                await element.SlideAndFadeOutAsync(AnimationSlideInDirection.Left, firstLoad ? 0 : 0.6f, keepMargin: true);
        }
    }

    /// <summary>
    /// Animates a framework element sliding it in from the right on show and sliding out to the right on hide.
    /// </summary>
    public class AnimateSlideInFromRightProperty : AnimateBaseProperty<AnimateSlideInFromRightProperty>
    {
        protected override async void DoAnimation(FrameworkElement element, bool value, bool firstLoad)
        {
            if (value) // Animate in
                await element.SlideAndFadeInAsync(AnimationSlideInDirection.Right, firstLoad, firstLoad ? 0 : 0.6f, keepMargin: false);
            else       // Animate out
                await element.SlideAndFadeOutAsync(AnimationSlideInDirection.Right, firstLoad ? 0 : 0.6f, keepMargin: false);
        }
    }

    /// <summary>
    /// Animates a framework element sliding it in from the right on show and sliding out to the right on hide.
    /// </summary>
    public class AnimateSlideInFromRightMarginProperty : AnimateBaseProperty<AnimateSlideInFromRightMarginProperty>
    {
        protected override async void DoAnimation(FrameworkElement element, bool value, bool firstLoad)
        {
            if (value) // Animate in
                await element.SlideAndFadeInAsync(AnimationSlideInDirection.Right, firstLoad, firstLoad ? 0 : 0.6f, keepMargin: true);
            else       // Animate out
                await element.SlideAndFadeOutAsync(AnimationSlideInDirection.Right, firstLoad ? 0 : 0.6f, keepMargin: true);
        }
    }

    /// <summary>
    /// Animates a framework element sliding up from the bottom on show and sliding out to the bottom on hide.
    /// </summary>
    public class AnimateSlideInFromBottomProperty : AnimateBaseProperty<AnimateSlideInFromBottomProperty>
    {
        protected override async void DoAnimation(FrameworkElement element, bool value, bool firstLoad)
        {
            if (value) // Animate in
                await element.SlideAndFadeInAsync(AnimationSlideInDirection.Bottom, firstLoad, firstLoad ? 0 : 0.6f, keepMargin: false);
            else       // Animate out
                await element.SlideAndFadeOutAsync(AnimationSlideInDirection.Bottom, firstLoad ? 0 : 0.6f, keepMargin: false);
        }
    }

    /// <summary>
    /// Animates a framework element sliding up from the bottom on load if the value is true.
    /// </summary>
    public class AnimateSlideInFromBottomOnLoadProperty : AnimateBaseProperty<AnimateSlideInFromBottomOnLoadProperty>
    {
        protected override async void DoAnimation(FrameworkElement element, bool value, bool firstLoad)
        {
            // Animate in
            await element.SlideAndFadeInAsync(AnimationSlideInDirection.Bottom, !value, !value ? 0 : 0.6f, keepMargin: false);
        }
    }

    /// <summary>
    /// Animates a framework element sliding up from the bottom on show and sliding out to the bottom on hide.
    /// </summary>
    /// <remarks>
    /// Retains the margin.
    /// </remarks>
    public class AnimateSlideInFromBottomMarginProperty : AnimateBaseProperty<AnimateSlideInFromBottomMarginProperty>
    {
        protected override async void DoAnimation(FrameworkElement element, bool value, bool firstLoad)
        {
            if (value) // Animate in
                await element.SlideAndFadeInAsync(AnimationSlideInDirection.Bottom, firstLoad, firstLoad ? 0 : 0.6f, keepMargin: true);
            else       // Animate out
                await element.SlideAndFadeOutAsync(AnimationSlideInDirection.Bottom, firstLoad ? 0 : 0.6f, keepMargin: true);
        }
    }

    /// <summary>
    /// Animates a framework element sliding up from the bottom on show and sliding out to the bottom on hide.
    /// </summary>
    public class AnimateSlideInFromTopProperty : AnimateBaseProperty<AnimateSlideInFromTopProperty>
    {
        protected override async void DoAnimation(FrameworkElement element, bool value, bool firstLoad)
        {
            if (value) // Animate in
                await element.SlideAndFadeInAsync(AnimationSlideInDirection.Top, firstLoad, firstLoad ? 0 : 0.6f, keepMargin: false);
            else       // Animate out
                await element.SlideAndFadeOutAsync(AnimationSlideInDirection.Top, firstLoad ? 0 : 0.6f, keepMargin: false);
        }
    }


    /// <summary>
    /// Animates a framework element fading in on show and fading out on hide.
    /// </summary>
    public class AnimateFadeInProperty : AnimateBaseProperty<AnimateFadeInProperty>
    {
        protected override async void DoAnimation(FrameworkElement element, bool value, bool firstLoad)
        {
            if (value) // Animate in
                await element.FadeInAsync(firstLoad, firstLoad ? 0 : 0.6f);
            else       // Animate out
                await element.FadeOutAsync(firstLoad ? 0 : 0.6f);
        }
    }

    /// <summary>
    /// Animates a framework element sliding it from right to left and repeating forever.
    /// </summary>
    public class AnimateMarqueeProperty : AnimateBaseProperty<AnimateMarqueeProperty>
    {
        protected override void DoAnimation(FrameworkElement element, bool value, bool firstLoad)
        {
            // Animate in
            element.MarqueeAsync(firstLoad ? 0 : 6f);
        }
    }

    /// <summary>
    /// Animates a framework element fading in on show and fading out on hide.
    /// </summary>
    public class AnimateBlurInProperty : AnimateBaseProperty<AnimateBlurInProperty>
    {
        protected override async void DoAnimation(FrameworkElement element, bool value, bool firstLoad)
        {
            if (value) // Animate in
                await element.BlurInAsync(FirstLoad ? 0 : 0.8f);
            else       // Animate out
                await element.BlurOutAsync(FirstLoad ? 0 : 0.4f);
        }
    }

    /*
    /// <summary>
    /// Animates a framework element on show.
    /// </summary>
    public class AnimateSlideInFromBottomProperty : AnimateBaseProperty<AnimateSlideInFromBottomProperty>
    {
        protected override async void DoAnimation(FrameworkElement element, bool value)
        {
            if (value)
                await element.SlideAndFadeInFromBottomAsync(FirstLoad ? 0f : 1.0f, keepMargin: false); //if first load then do it instantly
            else
                await element.SlideAndFadeOutToBottomAsync(FirstLoad ? 0f : 1.0f, keepMargin: false); //if first load then do it instantly
        }
    }

    /// <summary>
    /// Animates a framework element on show.
    /// </summary>
    public class AnimateSlideInFromBottomMarginProperty : AnimateBaseProperty<AnimateSlideInFromBottomMarginProperty>
    {
        protected override async void DoAnimation(FrameworkElement element, bool value)
        {
            if (value)
                await element.SlideAndFadeInFromBottomAsync(FirstLoad ? 0f : 1.0f, keepMargin: true); //if first load then do it instantly
            else
                await element.SlideAndFadeOutToBottomAsync(FirstLoad ? 0f : 1.0f, keepMargin: true); //if first load then do it instantly
        }
    }

    /// <summary>
    /// Animates a framework element fading in on show and fading out on hide.
    /// </summary>
    public class AnimateFadeInProperty : AnimateBaseProperty<AnimateFadeInProperty>
    {
        protected override async void DoAnimation(FrameworkElement element, bool value)
        {
            if (value) // Animate in
                await element.FadeInAsync(FirstLoad ? 0 : 0.5f);
            else // Animate out
                await element.FadeOutAsync(FirstLoad ? 0 : 0.5f); 
        }
    }
    */

}
