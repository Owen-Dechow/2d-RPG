import os


def add_namespace(directory, namespace):
    for filename in os.listdir(directory):
        if filename.endswith(".cs"):
            filepath = os.path.join(directory, filename)
            with open(filepath, "r+") as file:
                filedata = file.read()
                # Check if the file already has a namespace
                if "namespace" not in filedata:
                    # Add the namespace
                    filedata = "namespace " + namespace + "\n{\n" + filedata + "\n}"
                    # Write the file out again
                    file.seek(0)
                    file.write(filedata)
                    file.truncate()


# Usage
add_namespace(
    r"C:\Users\Owen\Desktop\Unity\Projects\2dRPG\Assets\Scripts\NPC\Nodes\NodeObjects",
    "NewNamespace",
)
