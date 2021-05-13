module Foo.Bar

open VoxelTycoon
open VoxelTycoon.Game.UI.ModernUI
open VoxelTycoon.Modding
open VoxelTycoon.Serialization
open VoxelTycoon.UI

open VoxelTycoon.Tracks
open UnityEngine
open VoxelTycoon.Game.UI
open VoxelTycoon.Tools

type OldSchoolStyleManager() as this =
    inherit Manager<OldSchoolStyleManager>() 

    let mutable _transforms: ResizeArray<Transform> option = None

    member val Enabled = true with get, set

    override self.OnLateUpdate() =
        base.OnLateUpdate()

        if this.Enabled then
            let companies = CompanyManager.Current.GetAll()
            for i=0 to companies.Count - 1 do
                let company = companies.[i]
                let trackUnits = TrackUnitManager.Current.GetAll company
                for j=0 to trackUnits.Count - 1 do
                    let trackUnit = trackUnits.[j]
                    for transform in trackUnit.GetComponentsInChildren<Transform>() do
                        let rotation = transform.localEulerAngles
                        transform.localEulerAngles <-
                                Vector3(Helper.RoundTo(rotation.x, 90f / 4f), Helper.RoundTo(rotation.y, 90f / 2f), Helper.RoundTo(rotation.z, 90f / 4f))


type OldSchoolStyleModSettingsTool() =
    let toogleHotkey = Hotkey KeyCode.Z
    let mutable toogleHotkeyPanelItem: HotkeyPanelItem option = None

    
    interface ITool with
        override _.Activate() =
            toogleHotkeyPanelItem <- Some <| HotkeyPanel.Current.Add("").AddKey(toogleHotkey)
        override _.OnUpdate() =
            if ToolHelper.IsHotkeyDown(toogleHotkey) then
                OldSchoolStyleManager.Current.Enabled <- not <| OldSchoolStyleManager.Current.Enabled

            toogleHotkeyPanelItem
            |> Option.iter (fun item -> item.SetCaption(if OldSchoolStyleManager.Current.Enabled then "Enabled" else "Disabled"))

            false

        override _.Deactivate _ =
            HotkeyPanel.Current.Clear()
            true


type TheMod() =
    inherit Mod()

    override _.OnGameStarting() =
        OldSchoolStyleManager.Initialize()
        ()
    override _.OnGameStarted() =
        Toolbar.Current.AddButton(FontIcon.Ketizoloto(I.Settings1), "Old school style settings", ToolToolbarAction(fun () -> OldSchoolStyleModSettingsTool() :> _));
        ()
    override _.Read(reader: StateBinaryReader) =
        ()
    override _.Write(writer: StateBinaryWriter) =
        ()
