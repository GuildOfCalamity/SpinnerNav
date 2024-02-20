using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace SpinnerNav
{
    /// <summary>
    /// The direction an animation slides in (the slide out direction is reversed)
    /// <summary>
    public enum AnimationSlideInDirection
    {
        /// <summary>
        /// Slide in from the left
        /// </summary>
        Left = 0,
        /// <summary>
        /// Slide in from the right
        /// </summary>
        Right = 1,
        /// <summary>
        /// Slide in from the top
        /// </summary>
        Top = 2,
        /// <summary>
        /// Slide in from the bottom
        /// </summary>
        Bottom = 3
    }

    public static class FrameworkElementAnimations
    {

        #region Slide In / Out

        /// <summary>
        /// Slides an element in
        /// </summary>
        /// <param name="element">The element to animate</param>
        /// <param name="direction">The direction of the slide</param>
        /// <param name="seconds">The time the animation will take</param>
        /// <param name="keepMargin">Whether to keep the element at the same width during animation</param>
        /// <param name="size">The animation width/height to animate to. If not specified the elements size is used</param>
        /// <param name="firstLoad">Indicates if this is the first load</param>
        /// <returns></returns>
        public static async Task SlideAndFadeInAsync(this FrameworkElement element, AnimationSlideInDirection direction, bool firstLoad, float seconds = 0.3f, bool keepMargin = true, int size = 0)
        {
            // Create the storyboard
            var sb = new Storyboard();

            // Slide in the correct direction
            switch (direction)
            {
                // Add slide from left animation
                case AnimationSlideInDirection.Left:
                    sb.AddSlideFromLeft(seconds, size == 0 ? element.ActualWidth : size, keepMargin: keepMargin);
                    break;
                // Add slide from right animation
                case AnimationSlideInDirection.Right:
                    sb.AddSlideFromRight(seconds, size == 0 ? element.ActualWidth : size, keepMargin: keepMargin);
                    break;
                // Add slide from top animation
                case AnimationSlideInDirection.Top:
                    sb.AddSlideFromTop(seconds, size == 0 ? element.ActualHeight : size, keepMargin: keepMargin);
                    break;
                // Add slide from bottom animation
                case AnimationSlideInDirection.Bottom:
                    sb.AddSlideFromBottom(seconds, size == 0 ? element.ActualHeight : size, keepMargin: keepMargin);
                    break;
            }
            // Add fade in animation
            sb.AddFadeIn(seconds);

            // Start animating
            sb.Begin(element);

            // Make page visible only if we are animating or its the first load
            if (seconds != 0 || firstLoad)
                element.Visibility = Visibility.Visible;

            // Wait for it to finish
            await Task.Delay((int)(seconds * 1000));
        }

        /// <summary>
        /// Slides an element out
        /// </summary>
        /// <param name="element">The element to animate</param>
        /// <param name="direction">The direction of the slide (this is for the reverse slide out action, so Left would slide out to left)</param>
        /// <param name="seconds">The time the animation will take</param>
        /// <param name="keepMargin">Whether to keep the element at the same width during animation</param>
        /// <param name="size">The animation width/height to animate to. If not specified the elements size is used</param>
        /// <returns></returns>
        public static async Task SlideAndFadeOutAsync(this FrameworkElement element, AnimationSlideInDirection direction, float seconds = 0.3f, bool keepMargin = true, int size = 0)
        {
            // Create the storyboard
            var sb = new Storyboard();

            // Slide in the correct direction
            switch (direction)
            {
                // Add slide to left animation
                case AnimationSlideInDirection.Left:
                    sb.AddSlideToLeft(seconds, size == 0 ? element.ActualWidth : size, keepMargin: keepMargin);
                    break;
                // Add slide to right animation
                case AnimationSlideInDirection.Right:
                    sb.AddSlideToRight(seconds, size == 0 ? element.ActualWidth : size, keepMargin: keepMargin);
                    break;
                // Add slide to top animation
                case AnimationSlideInDirection.Top:
                    sb.AddSlideToTop(seconds, size == 0 ? element.ActualHeight : size, keepMargin: keepMargin);
                    break;
                // Add slide to bottom animation
                case AnimationSlideInDirection.Bottom:
                    sb.AddSlideToBottom(seconds, size == 0 ? element.ActualHeight : size, keepMargin: keepMargin);
                    break;
            }

            // Add fade in animation
            sb.AddFadeOut(seconds);

            // Start animating
            sb.Begin(element);

            // Make page visible only if we are animating
            if (seconds != 0)
                element.Visibility = Visibility.Visible;

            // Wait for it to finish
            await Task.Delay((int)(seconds * 1000));

            /* 
                NOTE: This can cause an issue where the previous animation is not finished and the user
                starts a new animation, the direction reversal works correctly but the fade does not.

            // Make element invisible
            element.Visibility = Visibility.Hidden;
            */
        }

        #endregion

        #region Fade In / Out

        /// <summary>
        /// Fades an element in
        /// </summary>
        /// <param name="element">The element to animate</param>
        /// <param name="seconds">The time the animation will take</param>
        /// <param name="firstLoad">Indicates if this is the first load</param>
        /// <returns></returns>
        public static async Task FadeInAsync(this FrameworkElement element, bool firstLoad, float seconds = 0.3f)
        {
            // Create the storyboard
            var sb = new Storyboard();

            // Add fade in animation
            sb.AddFadeIn(seconds);

            // Start animating
            sb.Begin(element);

            // Make page visible only if we are animating or its the first load
            if (seconds != 0 || firstLoad)
                element.Visibility = Visibility.Visible;

            // Wait for it to finish
            await Task.Delay((int)(seconds * 1000));
        }

        /// <summary>
        /// Fades out an element
        /// </summary>
        /// <param name="element">The element to animate</param>
        /// <param name="seconds">The time the animation will take</param>
        /// <param name="firstLoad">Indicates if this is the first load</param>
        /// <returns></returns>
        public static async Task FadeOutAsync(this FrameworkElement element, float seconds = 0.3f)
        {
            // Create the storyboard
            var sb = new Storyboard();

            // Add fade in animation
            sb.AddFadeOut(seconds);

            // Start animating
            sb.Begin(element);

            // Make page visible only if we are animating or its the first load
            if (seconds != 0)
                element.Visibility = Visibility.Visible;

            // Wait for it to finish
            await Task.Delay((int)(seconds * 1000));

            // Fully hide the element
            element.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region Marquee

        /// <summary>
        /// Animates a marquee style element
        /// The structure should be:
        /// [Border ClipToBounds="True"]
        ///   [Border local:AnimateMarqueeProperty.Value="True"]
        ///      [Content HorizontalAlignment="Left"]
        ///   [/Border]
        /// [/Border]
        /// </summary>
        /// <param name="element">The element to animate</param>
        /// <param name="seconds">The time the animation will take</param>
        /// <returns></returns>
        public static void MarqueeAsync(this FrameworkElement element, float seconds = 3f)
        {
            // Create the storyboard
            var sb = new Storyboard();

            // Run until element is unloaded
            var unloaded = false;

            // Monitor for element unloading
            element.Unloaded += (s, e) => unloaded = true;

            // Run a loop off the caller thread
            Task.Run(async () =>
            {
                // While the element is still available, recheck the size
                // after every loop in case the container was resized
                while (element != null && !unloaded)
                {
                    // Create width variables.
                    var width = 0d;
                    var innerWidth = 0d;

                    try
                    {
                        // Check if element is still loaded.
                        if (element == null || unloaded)
                            break;

                        // Try and get current width.
                        width = element.ActualWidth;
                        innerWidth = ((element as Border).Child as FrameworkElement).ActualWidth;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"[WARNING] MarqueeAsync: {ex.Message}");
                        // Any issues then stop animating. (presume element destroyed)
                        break;
                    }

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        // Add marquee animation.
                        sb.AddMarquee(seconds, width, innerWidth);

                        // Start animating.
                        sb.Begin(element);

                        // Make page visible.
                        element.Visibility = Visibility.Visible;
                    });

                    // Wait for it to finish animating.
                    await Task.Delay((int)seconds * 1000);

                    // If this is from first load or zero seconds of animation, do not repeat.
                    if (seconds == 0)
                        break;
                }
            });
        }

        #endregion


        /* --- ORIGINAL ---
         
        /// <summary>
        /// Slides an element in from the right side of the screen
        /// </summary>
        /// <param name="element"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static async Task SlideAndFadeInFromRightAsync(this FrameworkElement element, float seconds = 0.8f, bool keepMargin = true, int width = 0)
        {
            var sb = new Storyboard();
  
            sb.AddSlideFromRight(seconds, width == 0 ? element.ActualWidth : width, keepMargin: keepMargin);
            
            sb.AddFadeIn(seconds <= 0.1 ? 1 : seconds);

            // Make page visible only if we are animating
            if (seconds != 0)
                element.Visibility = Visibility.Visible;

            sb.Begin(element);
            
            await Task.Delay((int)seconds * 1000);
        }

        /// <summary>
        /// Slides an element in from the left side of the screen
        /// </summary>
        /// <param name="element"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static async Task SlideAndFadeInFromLeftAsync(this FrameworkElement element, float seconds = 0.8f, bool keepMargin = true, int width = 0)
        {
            var sb = new Storyboard();

            sb.AddSlideFromLeft(seconds, width == 0 ? element.ActualWidth : width, keepMargin: keepMargin);

            sb.AddFadeIn(seconds <= 0.1 ? 1 : seconds);

            // Make page visible only if we are animating
            if (seconds != 0)
                element.Visibility = Visibility.Visible;

            sb.Begin(element);

            await Task.Delay((int)seconds * 1000);
        }

        /// <summary>
        /// Slides an element out to the left side of the screen
        /// </summary>
        /// <param name="element"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static async Task SlideAndFadeOutToLeftAsync(this FrameworkElement element, float seconds = 0.8f, bool keepMargin = true, int width = 0)
        {
            var sb = new Storyboard();

            sb.AddSlideToLeft(seconds, width == 0 ? element.ActualWidth : width, keepMargin: keepMargin);

            sb.AddFadeOut(seconds <= 0.1 ? 1 : seconds);

            // Make page visible only if we are animating
            if (seconds != 0)
                element.Visibility = Visibility.Visible;

            sb.Begin(element);

            await Task.Delay((int)seconds * 1000);

            // NOTE: We need to hide the element when finished, otherwise
            // the element will be on top of others but not be visible.
            element.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Slides an element out to the right side of the screen
        /// </summary>
        /// <param name="element"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static async Task SlideAndFadeOutToRightAsync(this FrameworkElement element, float seconds = 0.8f, bool keepMargin = true, int width = 0)
        {
            var sb = new Storyboard();

            sb.AddSlideToRight(seconds, width == 0 ? element.ActualWidth : width, keepMargin: keepMargin);

            sb.AddFadeOut(seconds <= 0.1 ? 1 : seconds);
            
            // Make page visible only if we are animating
            if (seconds != 0)
                element.Visibility = Visibility.Visible;

            sb.Begin(element);

            await Task.Delay((int)seconds * 1000);

            // NOTE: We need to hide the element when finished, otherwise
            // the element will be on top of others but not be visible.
            element.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Zoom an element into the screen
        /// </summary>
        /// <param name="element"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static async Task ZoomIn(this FrameworkElement element, float seconds = 0.8f, int width = 0)
        {
            var sb = new Storyboard();

            sb.AddZoomIn(seconds, width == 0 ? element.ActualWidth : width);

            sb.AddFadeIn(seconds <= 0.1 ? 1 : seconds);

            if (seconds == 0)
                element.Visibility = Visibility.Visible;

            sb.Begin(element);

            await Task.Delay((int)seconds * 1000);
        }



        /// <summary>
        /// Slides an element in from the bottom of the screen
        /// </summary>
        /// <param name="element"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static async Task SlideAndFadeInFromBottomAsync(this FrameworkElement element, float seconds = 0.8f, bool keepMargin = true, int height = 0)
        {
            var sb = new Storyboard();

            sb.AddSlideFromBottom(seconds, height == 0 ? element.ActualHeight : height, keepMargin: keepMargin);

            sb.AddFadeIn(seconds <= 0.1 ? 1 : seconds);

            // Make page visible only if we are animating
            if (seconds != 0)
                element.Visibility = Visibility.Visible;

            sb.Begin(element);

            await Task.Delay((int)seconds * 1000);
        }

        /// <summary>
        /// Slides an element out to the bottom of the screen
        /// </summary>
        /// <param name="element"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static async Task SlideAndFadeOutToBottomAsync(this FrameworkElement element, float seconds = 0.8f, bool keepMargin = true, int height = 0)
        {
            var sb = new Storyboard();

            sb.AddSlideToBottom(seconds, height == 0 ? element.ActualHeight : height, keepMargin: keepMargin);

            sb.AddFadeOut(seconds <= 0.1 ? 1 : seconds);

            // Make page visible only if we are animating
            if (seconds != 0)
                element.Visibility = Visibility.Visible;

            sb.Begin(element);


            await Task.Delay((int)seconds * 1000);

            // NOTE: We need to hide the element when finished, otherwise
            // the element will be on top of others but not be visible.
            element.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Fades an element into view
        /// </summary>
        /// <param name="element"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static async Task FadeInAsync(this FrameworkElement element, float seconds = 0.5f)
        {
            var sb = new Storyboard();

            sb.AddFadeIn(seconds <= 0.1 ? 1 : seconds);

            // Make page visible only if we are animating
            if (seconds != 0)
                element.Visibility = Visibility.Visible;

            sb.Begin(element);

            await Task.Delay((int)seconds * 1000);

            // Fully hide the element
            element.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Fades an element out of view
        /// </summary>
        /// <param name="element"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static async Task FadeOutAsync(this FrameworkElement element, float seconds = 0.5f)
        {
            var sb = new Storyboard();

            sb.AddFadeOut(seconds <= 0.1 ? 1 : seconds);

            element.Visibility = Visibility.Visible;

            sb.Begin(element);

            await Task.Delay((int)seconds * 1000);

            // NOTE: We need to hide the element when finished, otherwise
            // the element will be on top of others but not be visible.
            element.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Animates a marquee style element
        /// The structure should be:
        /// [Border ClipToBounds="True"]
        ///   [Border local:AnimateMarqueeProperty.Value="True"]
        ///      [Content HorizontalAlignment="Left"]
        ///   [/Border]
        /// [/Border]
        /// </summary>
        /// <param name="element">The element to animate</param>
        /// <param name="seconds">The time the animation will take</param>
        /// <returns></returns>
        public static void MarqueeAsync(this FrameworkElement element, float seconds = 6f)
        {
            // Create the storyboard
            var sb = new Storyboard();

            // Run until element is unloaded
            var unloaded = false;

            // Monitor for element unloading
            element.Unloaded += (s, e) => unloaded = true;

            // Run a loop off the caller thread
            IoC.Task.Run(async () =>
            {
                // While the element is still available, recheck the size
                // after every loop in case the container was resized
                while (element != null && !unloaded)
                {
                    // Create width variables
                    var width = 0d;
                    var innerWidth = 0d;

                    try
                    {
                        // Check if element is still loaded
                        if (element == null || unloaded)
                            break;

                        // Try and get current width
                        width = element.ActualWidth;
                        innerWidth = ((element as Border).Child as FrameworkElement).ActualWidth;
                    }
                    catch
                    {
                        // Any issues then stop animating (presume element destroyed)
                        break;
                    }

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        // Add marquee animation
                        sb.AddMarquee(seconds, width, innerWidth);

                        // Start animating
                        sb.Begin(element);

                        // Make page visible
                        element.Visibility = Visibility.Visible;
                    });

                    // Wait for it to finish animating
                    await Task.Delay((int)seconds * 1000);

                    // If this is from first load or zero seconds of animation, do not repeat
                    if (seconds == 0)
                        break;
                }
            });
        }

        */

        /// <summary>
        /// A blur effect on the way in. (tried adding this to StoryboardHelpers but it did not seem to work properly)
        /// </summary>
        /// <param name="element"><see cref="FrameworkElement"/></param>
        /// <param name="seconds">animation time in seconds</param>
        public static async Task BlurInAsync(this FrameworkElement element, float seconds = 0.8f)
        {
            var blur = new BlurEffect();
            blur.Radius = 12;
            blur.KernelType = KernelType.Gaussian;
            element.Effect = blur;
            DoubleAnimation da = new DoubleAnimation();
            da.From = 0;
            da.To = blur.Radius;
            da.Duration = new Duration(TimeSpan.FromSeconds(seconds));
            blur.BeginAnimation(BlurEffect.RadiusProperty, da);

            await Task.Delay((int)seconds * 1000);
        }

        /// <summary>
        /// A blur effect on the way out. (tried adding this to StoryboardHelpers but it did not seem to work properly)
        /// </summary>
        /// <param name="element"><see cref="FrameworkElement"/></param>
        /// <param name="seconds">animation time in seconds</param>
        public static async Task BlurOutAsync(this FrameworkElement element, float seconds = 0.8f)
        {
            var blur = new BlurEffect();
            blur.Radius = 12;
            blur.KernelType = KernelType.Gaussian;
            element.Effect = blur;
            DoubleAnimation da = new DoubleAnimation();
            da.From = blur.Radius;
            da.To = 0;
            da.Duration = new Duration(TimeSpan.FromSeconds(seconds));
            blur.BeginAnimation(BlurEffect.RadiusProperty, da);

            await Task.Delay((int)seconds * 1000);

            // Make sure we remove any effects from the element (this will immeadiately remove the blur)
            //element.Effect = null;
        }
    }
}
