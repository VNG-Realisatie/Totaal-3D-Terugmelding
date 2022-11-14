
namespace T3D.Uitbouw.BoundaryFeatures
{
    public class SelectComponent : SelectableLibraryItem
    {
        public float ComponentWidth;
        public float ComponentHeight;

        public BoundaryFeature ComponentObject;

        protected override void OnLibraryItemSelected()
        {
            LibraryComponentSelectedEvent.RaiseComponentSelected(this, DragContainerImage, IsTopComponent, ComponentWidth, ComponentHeight, ComponentObject, this);
        }
    }
}
