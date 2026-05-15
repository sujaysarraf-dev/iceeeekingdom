import os
import subprocess

def run_git(args):
    print(f"Running: git {' '.join(args)}")
    result = subprocess.run(['git'] + args, capture_output=True, text=True)
    if result.returncode != 0:
        print(f"Error: {result.stderr}")
    return result.returncode == 0

def chunked_add_and_commit():
    # Unstage everything
    run_git(['reset'])
    
    # Add files one by one or in small groups
    # We can start with ProjectSettings and small root files
    root_files = [f for f in os.listdir('.') if os.path.isfile(f) and f not in ['find_large_files.py', 'chunk_commit.py']]
    for f in root_files:
        run_git(['add', f])
    run_git(['add', 'ProjectSettings'])
    run_git(['add', 'Packages'])
    run_git(['commit', '-m', "Commit: Root files and settings"])
    
    # Now Assets
    assets_path = 'Assets'
    if os.path.exists(assets_path):
        subdirs = os.listdir(assets_path)
        for subdir in subdirs:
            subdir_path = os.path.join(assets_path, subdir)
            print(f"Adding {subdir_path}...")
            run_git(['add', subdir_path])
            run_git(['commit', '-m', f"Commit: Assets/{subdir}"])

if __name__ == "__main__":
    chunked_add_and_commit()
