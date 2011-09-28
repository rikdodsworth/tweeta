
namespace Tweeta.ViewModel.Items
{
    public class AnimatedTileViewModel : GenericListItem
    {

        public AnimatedTileViewModel()
        {
        }


        private string frontImage;
        public string FrontImage
        {
            get
            {
                return frontImage;
            }
            set
            {
                if (frontImage != value)
                {
                    frontImage = value;
                    RaisePropertyChanged("FrontImage");
                }
            }
        }
        private string backImage;
        public string BackImage
        {
            get
            {
                return backImage;
            }
            set
            {
                if (backImage != value)
                {
                    backImage = value;
                    RaisePropertyChanged("BackImage");
                }
            }
        }

        private string backContent;
        public string BackContent
        {
            get
            {
                return backContent;
            }
            set
            {
                if (backContent != value)
                {
                    backContent = value;
                    RaisePropertyChanged("BackContent");
                }
            }
        }
        private string backTitle;
        public string BackTitle
        {
            get
            {
                return backTitle;
            }
            set
            {
                if (backTitle != value)
                {
                    backTitle = value;
                    RaisePropertyChanged("BackTitle");
                }
            }
        }
        private string frontTitle;
        public string FrontTitle
        {
            get
            {
                return frontTitle;
            }
            set
            {
                if (frontTitle != value)
                {
                    frontTitle = value;
                    RaisePropertyChanged("FrontTitle");
                }
            }
        }

        private string frontContent;
        public string FrontContent
        {
            get
            {
                return frontContent;
            }
            set
            {
                if (frontContent != value)
                {
                    frontContent = value;
                    RaisePropertyChanged("FrontContent");
                }
            }
        }
    }
}
