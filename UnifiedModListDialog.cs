using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.CoreUI;
using Timberborn.Localization;
using Timberborn.Modding;
using Timberborn.SaveMetadataSystem;
using Timberborn.SingletonSystem;
using Timberborn.Versioning;
using UnityEngine;
using UnityEngine.UIElements;

namespace Calloatti.LoadGameModValidator
{
  public class UnifiedModListDialog : ILoadableSingleton
  {
    public static UnifiedModListDialog Instance;

    private readonly ILoc _loc;
    private readonly ModRepository _modRepository;
    private readonly DialogBoxShower _dialogBoxShower;

    public UnifiedModListDialog(ILoc loc, ModRepository modRepository, DialogBoxShower dialogBoxShower)
    {
      _loc = loc;
      _modRepository = modRepository;
      _dialogBoxShower = dialogBoxShower;
    }

    public void Load()
    {
      Instance = this;
    }

    public void ShowDialog(SaveMetadata metadata, Action continueCallback)
    {
      VisualElement root = new VisualElement();
      root.style.width = 1100;
      root.style.paddingBottom = 15;

      // --- TOP BAR (Warning + Toggle) ---
      VisualElement topBar = new VisualElement();
      topBar.style.alignItems = Align.Center;
      topBar.style.justifyContent = Justify.Center;
      topBar.style.marginBottom = 10;

      Label infoLabel = new Label(_loc.T("Calloatti.LoadGameModValidator.Warning"));
      infoLabel.AddToClassList("text--default");
      infoLabel.AddToClassList("text--centered");
      topBar.Add(infoLabel);

      VisualElement toggleContainer = new VisualElement();
      toggleContainer.style.flexDirection = FlexDirection.Row;
      toggleContainer.style.alignItems = Align.Center;
      toggleContainer.style.position = Position.Absolute;
      toggleContainer.style.right = 0;

      Label toggleLabel = new Label(_loc.T("Calloatti.LoadGameModValidator.ShowAll"));
      toggleLabel.style.color = new Color(0.8f, 0.8f, 0.8f);
      toggleLabel.style.marginRight = 10;
      toggleContainer.Add(toggleLabel);

      Toggle showAllToggle = new Toggle();
      showAllToggle.AddToClassList("game-toggle");
      showAllToggle.value = false;
      toggleContainer.Add(showAllToggle);

      topBar.Add(toggleContainer);
      root.Add(topBar);

      // FIXED HEADER
      // FIX: Swapped SavedVersion and CurrentVersion
      VisualElement headerRow = CreateRow(
          _loc.T("Calloatti.LoadGameModValidator.Column.Name"),
          _loc.T("Calloatti.LoadGameModValidator.Column.Id"),
          _loc.T("Calloatti.LoadGameModValidator.Column.SavedVersion"),
          _loc.T("Calloatti.LoadGameModValidator.Column.CurrentVersion"),
          _loc.T("Calloatti.LoadGameModValidator.Column.Status"),
          Color.white, true
      );
      headerRow.style.marginTop = 15;
      headerRow.style.paddingRight = 20;
      root.Add(headerRow);

      // SCROLL VIEW
      ScrollView scrollView = new ScrollView();
      scrollView.style.marginBottom = 15;
      scrollView.style.height = 440;
      scrollView.style.flexGrow = 1;

      scrollView.verticalScrollerVisibility = ScrollerVisibility.AlwaysVisible;
      scrollView.horizontalScrollerVisibility = ScrollerVisibility.Hidden;

      // --- SCROLLBAR STYLING ---
      var dragger = scrollView.Q<VisualElement>(className: "unity-base-slider__dragger");
      if (dragger != null)
      {
        dragger.style.width = 20;
        dragger.style.minHeight = 58;
        dragger.style.backgroundColor = Color.clear;
        dragger.style.borderTopWidth = 0;
        dragger.style.borderBottomWidth = 0;
        dragger.style.borderLeftWidth = 0;
        dragger.style.borderRightWidth = 0;

        var tex = Resources.Load<Texture2D>("UI/Images/Core/vertical-scroll-button-nine-slice");
        if (tex != null)
        {
          dragger.style.backgroundImage = new StyleBackground(tex);
          dragger.style.unitySliceTop = 14;
          dragger.style.unitySliceBottom = 14;
          dragger.style.unitySliceLeft = 14;
          dragger.style.unitySliceRight = 14;
        }
      }

      var tracker = scrollView.Q<VisualElement>(className: "unity-base-slider__tracker");
      if (tracker != null)
      {
        tracker.style.width = 20;
        tracker.style.backgroundColor = Color.clear;
        tracker.style.borderTopWidth = 0;
        tracker.style.borderBottomWidth = 0;
        tracker.style.borderLeftWidth = 0;
        tracker.style.borderRightWidth = 0;

        var tex = Resources.Load<Texture2D>("UI/Images/Core/vertical-scroll-bar-nine-slice");
        if (tex != null)
        {
          tracker.style.backgroundImage = new StyleBackground(tex);
          tracker.style.unitySliceTop = 16;
          tracker.style.unitySliceBottom = 16;
        }
      }

      // --- FILTERING LOGIC ---
      List<VisualElement> matchingRows = new List<VisualElement>();
      var rowsData = GenerateUnifiedList(metadata);

      foreach (var row in rowsData)
      {
        // FIX: Swapped row.SavedVersion and row.CurrentVersion
        VisualElement rowElement = CreateRow(row.Name, row.Id, row.SavedVersion, row.CurrentVersion, _loc.T(row.StatusKey), row.StatusColor, false);

        if (row.StatusKey == "Calloatti.LoadGameModValidator.Status.Match")
        {
          rowElement.style.display = DisplayStyle.None;
          matchingRows.Add(rowElement);
        }

        scrollView.Add(rowElement);
      }

      showAllToggle.RegisterValueChangedCallback(evt =>
      {
        DisplayStyle newDisplay = evt.newValue ? DisplayStyle.Flex : DisplayStyle.None;
        foreach (var matchRow in matchingRows)
        {
          matchRow.style.display = newDisplay;
        }
      });

      root.Add(scrollView);

      Label confirmLabel = new Label(_loc.T("Calloatti.LoadGameModValidator.Confirmation"));
      confirmLabel.AddToClassList("text--default");
      confirmLabel.AddToClassList("text--centered");
      root.Add(confirmLabel);

      _dialogBoxShower.Create()
          .AddContent(root)
          .SetMaxWidth(1200)
          .SetConfirmButton(continueCallback)
          .SetDefaultCancelButton()
          .Show();
    }

    private class ModEntryData
    {
      public string Name, Id, CurrentVersion, SavedVersion, StatusKey;
      public Color StatusColor;
    }

    private List<ModEntryData> GenerateUnifiedList(SaveMetadata metadata)
    {
      List<ModEntryData> entries = new List<ModEntryData>();
      List<Mod> activeMods = _modRepository.EnabledMods.ToList();
      List<Mod> allMods = _modRepository.Mods.ToList();

      if (metadata != null && metadata.Mods != null)
      {
        foreach (var savedMod in metadata.Mods)
        {
          var entry = new ModEntryData
          {
            Name = savedMod.Name,
            Id = savedMod.Id,
            SavedVersion = Timberborn.Versioning.Version.Create(savedMod.Version).Formatted,
            CurrentVersion = "-"
          };

          var activeMatch = activeMods.FirstOrDefault(m => m.Manifest.Id == savedMod.Id);
          if (activeMatch != null)
          {
            entry.CurrentVersion = activeMatch.Manifest.Version.Formatted;

            if (activeMatch.Manifest.Version.Full == savedMod.Version)
            {
              entry.StatusKey = "Calloatti.LoadGameModValidator.Status.Match";
              entry.StatusColor = Color.white;
            }
            else
            {
              entry.StatusKey = "Calloatti.LoadGameModValidator.Status.Mismatch";
              entry.StatusColor = new Color(1.0f, 0.8f, 0.2f);
            }
            activeMods.Remove(activeMatch);
          }
          else
          {
            var inactiveMatch = allMods.FirstOrDefault(m => m.Manifest.Id == savedMod.Id);
            if (inactiveMatch != null)
            {
              entry.CurrentVersion = inactiveMatch.Manifest.Version.Formatted;
              entry.StatusKey = "Calloatti.LoadGameModValidator.Status.Disabled";
              entry.StatusColor = new Color(1.0f, 0.55f, 0.0f);
            }
            else
            {
              entry.StatusKey = "Calloatti.LoadGameModValidator.Status.Missing";
              entry.StatusColor = new Color(0.9f, 0.3f, 0.3f);
            }
          }

          entries.Add(entry);
        }
      }

      foreach (var active in activeMods)
      {
        entries.Add(new ModEntryData
        {
          Name = active.DisplayName,
          Id = active.Manifest.Id,
          CurrentVersion = active.Manifest.Version.Formatted,
          SavedVersion = "-",
          StatusKey = "Calloatti.LoadGameModValidator.Status.New",
          StatusColor = new Color(0.4f, 0.9f, 0.4f)
        });
      }

      return entries.OrderBy(e => e.Name).ToList();
    }

    // FIX: Swapped parameters to match the new order (savVer, then curVer)
    private VisualElement CreateRow(string name, string id, string savVer, string curVer, string status, Color color, bool isHeader)
    {
      VisualElement row = new VisualElement();
      row.style.flexDirection = FlexDirection.Row;
      row.style.paddingTop = 6;
      row.style.paddingBottom = 6;
      row.style.borderBottomWidth = 1;
      row.style.borderBottomColor = new StyleColor(new Color(0.3f, 0.3f, 0.3f, 0.5f));
      row.style.paddingLeft = 8;

      if (isHeader)
      {
        row.style.borderBottomWidth = 2;
        row.style.unityFontStyleAndWeight = FontStyle.Bold;
      }

      row.Add(CreateCell(name, 280, color));
      row.Add(CreateCell(id, 280, color));
      row.Add(CreateCell(savVer, 145, color)); // Saved version added first
      row.Add(CreateCell(curVer, 145, color)); // Current version added second
      row.Add(CreateCell(status, 120, color));

      return row;
    }

    private Label CreateCell(string text, float width, Color color)
    {
      Label lbl = new Label(text);
      lbl.AddToClassList("text--default");
      lbl.style.width = width;
      lbl.style.color = color;
      lbl.style.unityTextAlign = TextAnchor.MiddleLeft;
      lbl.style.whiteSpace = WhiteSpace.NoWrap;
      lbl.style.overflow = Overflow.Hidden;
      lbl.style.textOverflow = TextOverflow.Ellipsis;
      return lbl;
    }
  }
}