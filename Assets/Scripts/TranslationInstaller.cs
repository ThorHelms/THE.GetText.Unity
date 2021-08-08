using Zenject;

namespace Assets.Scripts
{
    public class TranslationInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);

            Container.DeclareSignal<LanguageChangedSignal>().OptionalSubscriber();

            Container.BindInterfacesTo<TranslationManager>().AsSingle();
        }
    }
}