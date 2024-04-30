@tool
extends EditorPlugin

const AUTOLOAD_NAME = "SolaceRuntimeAutoload"

func _enter_tree():
	add_autoload_singleton(AUTOLOAD_NAME,"res://addons/solace_core_plugin/core/SolaceRuntimeAutoload.tscn")
	
	
func _exit_tree():
	remove_autoload_singleton(AUTOLOAD_NAME)
	
