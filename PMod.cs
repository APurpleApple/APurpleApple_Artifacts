using HarmonyLib;
using Microsoft.Extensions.Logging;
using Nanoray.PluginManager;
using Nickel;
using System;
using System.Collections.Generic;
using System.Collections.Frozen;
using Shockah.Shared;
using Nanoray.EnumByNameSourceGenerator;
using Microsoft.Xna.Framework.Graphics;
using APurpleApple.GenericArtifacts.Cards;

namespace APurpleApple.GenericArtifacts;

public sealed class PMod : SimpleMod
{
    internal static PMod Instance { get; private set; } = null!;
    internal ILocalizationProvider<IReadOnlyList<string>> AnyLocalizations { get; }
    internal ILocaleBoundNonNullLocalizationProvider<IReadOnlyList<string>> Localizations { get; }

    public static Dictionary<string, ISpriteEntry> sprites = new();
    public static Dictionary<string, IStatusEntry> statuses = new();
    public static Dictionary<string, IPartEntry> parts = new();
    public static Dictionary<string, TTGlossary> glossaries = new();
    public static Dictionary<string, ICharacterAnimationEntry> animations = new();
    public static Dictionary<string, IArtifactEntry> artifacts = new();
    public static Dictionary<string, ICardEntry> cards = new();
    public static Dictionary<string, ICharacterEntry> characters = new();
    public static Dictionary<string, IShipEntry> ships = new();
    public static Dictionary<string, IDeckEntry> decks = new();

    public static BlendState multiplyBlend = new BlendState()
    {
        ColorBlendFunction = BlendFunction.Add,
        ColorSourceBlend = Blend.DestinationColor,
        ColorDestinationBlend = Blend.InverseSourceAlpha,
    };
    internal static IReadOnlyList<Type> Registered_Card_Types { get; } = [
        typeof(CardBumpersRamm),
        //typeof(CardPrinter)
    ];

    internal static IReadOnlyList<Type> Registered_Artifact_Types { get; } = [
        typeof(ArtifactGlassEngine),
        typeof(ArtifactGlassShield),
        typeof(ArtifactPainter),
        typeof(ArtifactSpikeBumper),
        typeof(ArtifactPrism),
        typeof(ArtifactPrinter),
        typeof(ArtifactPingPong),
        typeof(ArtifactRecycler),
        typeof(ArtifactSpinnyTop)
    ];

    public void RegisterSprite(string key, string fileName, IPluginPackage<IModManifest> package)
    {
        sprites.Add(key, Helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/" + fileName)));
    }

    private void Patch()
    {
        Harmony harmony = new("APurpleApple.GenericArtifacts");
        harmony.PatchAll();

        CustomTTGlossary.ApplyPatches(harmony);
    }

    public override object? GetApi(IModManifest requestingMod)
    {
        return new ApiImplementation();
    }

    public PMod(IPluginPackage<IModManifest> package, IModHelper helper, ILogger logger) : base(package, helper, logger)
    {
        Instance = this;

        Patch();

        this.AnyLocalizations = new JsonLocalizationProvider(
            tokenExtractor: new SimpleLocalizationTokenExtractor(),
            localeStreamFunction: locale => package.PackageRoot.GetRelativeFile($"i18n/{locale}.json").OpenRead()
        );
        this.Localizations = new MissingPlaceholderLocalizationProvider<IReadOnlyList<string>>(
            new CurrentLocaleOrEnglishLocalizationProvider<IReadOnlyList<string>>(this.AnyLocalizations)
        );

        RegisterSprite("ArtifactGlassEngine", "Artifacts/artifact_glassengine.png", package);
        RegisterSprite("ArtifactGlassEngineOff", "Artifacts/artifact_glassengine_off.png", package);
        RegisterSprite("ArtifactGlassShield", "Artifacts/artifact_glassshield.png", package);
        RegisterSprite("ArtifactGlassShieldOff", "Artifacts/artifact_glassshield_off.png", package);
        RegisterSprite("ArtifactPainter", "Artifacts/artifact_painterspalette.png", package);
        RegisterSprite("ArtifactPainterTick", "Artifacts/artifact_painter_tick.png", package);
        RegisterSprite("ArtifactRecycler", "Artifacts/artifact_recycler.png", package);
        RegisterSprite("ArtifactRecyclerOff", "Artifacts/artifact_recycler_off.png", package);
        RegisterSprite("ArtifactPrinter", "Artifacts/artifact_printer.png", package);
        RegisterSprite("ArtifactPrinterOff", "Artifacts/artifact_printer_off.png", package);
        RegisterSprite("ArtifactPingPong", "Artifacts/artifact_pingpongarmor.png", package);
        RegisterSprite("ArtifactBumper", "Artifacts/SpikeBumpers.png", package);
        RegisterSprite("ArtifactPrism", "Artifacts/artifact_colorfulCrystal.png", package);
        RegisterSprite("ArtifactSpinnyTop", "Artifacts/artifact_spinnytop.png", package);

        RegisterSprite("CardPrinter", "Cards/card_printer.png", package);
        RegisterSprite("CardRamm", "Cards/card_ramm.png", package);
        RegisterSprite("BorderRamm", "Cards/border_ramm.png", package);
        RegisterSprite("CardOverPaintStainA", "Cards/paintStainA.png", package);
        RegisterSprite("CardOverPaintStainB", "Cards/paintStainB.png", package);
        RegisterSprite("CardOverPaintStainC", "Cards/paintStainC.png", package);

        RegisterSprite("StatusPaint", "Icons/status_paint.png", package);
        RegisterSprite("ActionRamm", "Icons/hurtenemyicon.png", package);


        decks.Add("Ramm",Helper.Content.Decks.RegisterDeck("Ramm", new DeckConfiguration() { BorderSprite = sprites["BorderRamm"].Sprite, DefaultCardArt = SSpr.cards_Trash, Definition = new DeckDef() {color = Colors.white } }));


        foreach (var cardType in Registered_Card_Types)
            AccessTools.DeclaredMethod(cardType, nameof(IModCard.Register))?.Invoke(null, [helper]);

        foreach (var artifactType in Registered_Artifact_Types)
            AccessTools.DeclaredMethod(artifactType, nameof(IModArtifact.Register))?.Invoke(null, [helper]);

        helper.Events.OnModLoadPhaseFinished += (object? sender, ModLoadPhase e) => {
            if (e == ModLoadPhase.AfterDbInit)
            {
                foreach (var artifactType in Registered_Artifact_Types)
                    AccessTools.DeclaredMethod(artifactType, nameof(IModArtifact.AfterDBInit))?.Invoke(null, [helper]);
            }
        };
    }

}
