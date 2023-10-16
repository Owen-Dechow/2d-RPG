"""
This script finds and lists any extra npc behavior
trees it finds so that they can be removed.

When prompted select the assets folder.
"""

from tkinter import filedialog, Tk, messagebox
import os
from fnmatch import fnmatch
from typing import List


TREE_PATH = "Prefabs/NPCs/Trees"
SCENE_PATH = "Scenes"


def get_path():
    Tk().withdraw()  # prevents an empty tkinter window from appearing
    return filedialog.askdirectory()


def find_extras(trees, scenes):
    extras = []

    for tree in trees:
        with open(tree, "r") as tree_f:
            guid = None
            for l in tree_f.readlines():
                if "guid:" in l:
                    guid = l.split(":")[-1].strip()

            found = False
            for scene in scenes:
                with open(scene, "r") as scene_f:
                    if guid in scene_f.read():
                        found = True

        if not found:
            extras.append(tree)

    return extras


def get_trees(tree_path):
    trees = []
    for path, _, files in os.walk(tree_path):
        for name in files:
            if fnmatch(name, "*.asset.meta"):
                trees.append(os.path.join(path, name))

    return trees


def get_scenes(scenes_path):
    scenes = []
    for path, _, files in os.walk(scenes_path):
        for name in files:
            if fnmatch(name, "*.unity"):
                scenes.append(os.path.join(path, name))

    return scenes


def main():
    print("SELECT ASSETS FOLDER")
    assets_path = get_path()
    trees = get_trees(os.path.join(assets_path, TREE_PATH))
    scenes = get_scenes(os.path.join(assets_path, SCENE_PATH))

    extras: List[str] = find_extras(trees, scenes)
    if len(extras) == 0:
        none_found_message()
        return

    if get_delete_unused(extras):
        for f in extras:
            os.remove(f)
            os.remove(f.removesuffix(".meta"))


def get_delete_unused(extras):
    Tk().withdraw()
    return (
        messagebox.askquestion(
            "Delete Unused Trees?",
            "\n".join(e.removesuffix(".asset.meta") for e in extras),
        )
        == "yes"
    )


def none_found_message():
    Tk().withdraw()
    messagebox.showinfo("", "No unused trees found")


if __name__ == "__main__":
    main()
