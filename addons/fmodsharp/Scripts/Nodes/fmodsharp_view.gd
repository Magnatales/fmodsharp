@tool
extends Control

@export var SelectFolder: Button
@export var FetchBanks: Button

@export var FolderLabel: Label
@export var BankLabel: Label
@export var tree: Tree

@export var CopyPathButton: Button
@export var CopyGuidButton: Button

@export var PlayButton: Button
@export var StopButton: Button

@export var RightPanel: ScrollContainer

@export var EventPathLabel: Label
@export var EventGuidLabel: Label

@export var AutoFetchOnFocus: CheckBox
@export var Debug: CheckBox

func _enter_tree():
	var theme = EditorInterface.get_editor_theme()
	var copy_icon = theme.get_icon("ActionCopy", "EditorIcons")
	CopyPathButton.icon = copy_icon
	CopyGuidButton.icon = copy_icon

	var play_icon = theme.get_icon("Play", "EditorIcons")
	PlayButton.icon = play_icon

	var stop_icon = theme.get_icon("Stop", "EditorIcons")
	StopButton.icon = stop_icon
