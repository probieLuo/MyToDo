using MaterialDesignColors;
using MaterialDesignColors.ColorManipulation;
using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MyToDo.ViewModels
{
    public class SkinViewModel : BindableBase
    {
        private bool _isDarkTheme = true;
        public bool IsDarkTheme
        {
            get => _isDarkTheme;
            set
            {
                if (SetProperty(ref _isDarkTheme, value))
                {
                    ModifyTheme(theme => theme.SetBaseTheme(value ? Theme.Dark : Theme.Light));
                }
            }
        }

        private static void ModifyTheme(Action<ITheme> modificationAction)
        {
            var paletteHelper = new PaletteHelper();
            ITheme theme = paletteHelper.GetTheme();

            modificationAction?.Invoke(theme);

            paletteHelper.SetTheme(theme);
        }

        public IEnumerable<ISwatch> Swatches { get; } = SwatchHelper.Swatches;

        public DelegateCommand<object> ChangeHueCommand { get; private set; }

        private readonly PaletteHelper _paletteHelper = new();


        public SkinViewModel()
        {
            ChangeHueCommand = new DelegateCommand<object>(ChangeHue);
        }

        private void ChangeHue(object? obj)
        {
            var hue = (Color)obj!;

            //SelectedColor = hue;

            _paletteHelper.ChangePrimaryColor(hue);
            //_primaryColor = hue;
            //_primaryForegroundColor = _paletteHelper.GetTheme().PrimaryMid.GetForegroundColor();
            //if (ActiveScheme == ColorScheme.Primary)
            //{
            //    _paletteHelper.ChangePrimaryColor(hue);
            //    _primaryColor = hue;
            //    _primaryForegroundColor = _paletteHelper.GetTheme().PrimaryMid.GetForegroundColor();
            //}
            //else if (ActiveScheme == ColorScheme.Secondary)
            //{
            //    _paletteHelper.ChangeSecondaryColor(hue);
            //    _secondaryColor = hue;
            //    _secondaryForegroundColor = _paletteHelper.GetTheme().SecondaryMid.GetForegroundColor();
            //}
            //else if (ActiveScheme == ColorScheme.PrimaryForeground)
            //{
            //    SetPrimaryForegroundToSingleColor(hue);
            //    _primaryForegroundColor = hue;
            //}
            //else if (ActiveScheme == ColorScheme.SecondaryForeground)
            //{
            //    SetSecondaryForegroundToSingleColor(hue);
            //    _secondaryForegroundColor = hue;
            //}
        }
    }


    public static class PaletteHelperExtensions
    {
        public static void ChangePrimaryColor(this PaletteHelper paletteHelper, Color color)
        {
            Theme theme = (Theme)paletteHelper.GetTheme();

            theme.PrimaryLight = new ColorPair(color.Lighten());
            theme.PrimaryMid = new ColorPair(color);
            theme.PrimaryDark = new ColorPair(color.Darken());

            paletteHelper.SetTheme(theme);
        }

        public static void ChangeSecondaryColor(this PaletteHelper paletteHelper, Color color)
        {
            Theme theme = (Theme)paletteHelper.GetTheme();

            theme.SecondaryLight = new ColorPair(color.Lighten());
            theme.SecondaryMid = new ColorPair(color);
            theme.SecondaryDark = new ColorPair(color.Darken());

            paletteHelper.SetTheme(theme);
        }
    }


}
