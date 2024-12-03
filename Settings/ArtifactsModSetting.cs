using FSPRO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace APurpleApple.GenericArtifacts;

public sealed class ArtifactsModSetting : IModSettingsApi.IModSetting
{
    public UIKey Key { get; private set; }
    public event IModSettingsApi.OnMenuOpen? OnMenuOpen;
    public event IModSettingsApi.OnMenuClose? OnMenuClose;
    private IModSettingsApi.IModSettingsRoute CurrentRoute = null!;

    public required Func<string> Title { get; set; }
    public required Func<List<Type>> AllArtifacts { get; set; }
    public required Func<Type, bool> IsEnabled { get; set; }
    public required Action<IModSettingsApi.IModSettingsRoute, Type, bool> SetEnabled { get; set; }
    public Func<IEnumerable<Tooltip>>? Tooltips { get; set; }

    private UIKey BaseArtifactKey;

    public ArtifactsModSetting()
    {
        this.OnMenuOpen += (_, route) =>
        {
            if (this.Key == 0)
                this.Key = PMod.Instance.Helper.Utilities.ObtainEnumCase<UK>();
            if (this.BaseArtifactKey == 0)
                this.BaseArtifactKey = PMod.Instance.Helper.Utilities.ObtainEnumCase<UK>();
            this.CurrentRoute = route;
        };
    }

    ~ArtifactsModSetting()
    {
        if (this.Key != 0)
            PMod.Instance.Helper.Utilities.FreeEnumCase(this.Key.k);
        if (this.BaseArtifactKey != 0)
            PMod.Instance.Helper.Utilities.FreeEnumCase(this.BaseArtifactKey.k);
    }

    public void RaiseOnMenuOpen(G g, IModSettingsApi.IModSettingsRoute route)
        => this.OnMenuOpen?.Invoke(g, route);

    public void RaiseOnMenuClose(G g)
        => this.OnMenuClose?.Invoke(g);

    public Vec? Render(G g, Box box, bool dontDraw)
    {
        var width = 18;
        var height = 18;
        var perRow = (int)((box.rect.w - 20) / width);
        var rows = this.AllArtifacts().Chunk(perRow).ToList();

        if (!dontDraw)
        {
            Draw.Text(this.Title(), box.rect.x + 10, box.rect.y + 5, DB.thicket, Colors.textMain);

            for (var y = 0; y < rows.Count; y++)
            {
                var row = rows[y];
                for (var x = 0; x < row.Length; x++)
                {
                    Type artType = row[x];
                    Artifact art = Activator.CreateInstance(artType) as Artifact ?? new Artifact();
                    art.Render(g, new Vec(10 + x * width, 20 + y * height), false, true, false);
                    if (!IsEnabled(artType))
                    {
                        Draw.Sprite(PMod.sprites["SettingsDisabled"].Sprite, box.rect.x + 8 + x * width, box.rect.y + 18 + y * height);
                    }
                    Box? artBox = g.boxes.Find(b=>b.key == art.UIKey());
                    if (artBox != null)
                    {
                        artBox.onMouseDown = new MouseDownHandler(() =>
                        {
                            Audio.Play(Event.Click);
                            SetEnabled(this.CurrentRoute, artType, !IsEnabled(artType));
                        });
                    }
                }
            }

            if (this.Tooltips is { } tooltips && box.key is not null && box.IsHover())
                g.tooltips.Add(new Vec(box.rect.x2 - Tooltip.WIDTH, box.rect.y2), tooltips());
        }

        return new(box.rect.w, 20 + rows.Count * height);
    }
}