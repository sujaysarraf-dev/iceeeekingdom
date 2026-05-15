import os
import subprocess
import sys

def run_git(args):
    print(f"Running: git {' '.join(args)}")
    result = subprocess.run(['git'] + args, capture_output=True, text=True)
    if result.returncode != 0:
        print(f"Error: {result.stderr}")
        return False
    return True

def chunked_push():
    # 1. Reset to origin/main but keep files
    if not run_git(['reset', 'origin/main']):
        print("Failed to reset to origin/main")
        return

    # 2. Add and commit root files
    root_files = [f for f in os.listdir('.') if os.path.isfile(f) and f not in ['find_large_files.py', 'chunk_commit.py', 'chunk_push.py']]
    for f in root_files:
        run_git(['add', f])
    run_git(['add', 'ProjectSettings'])
    run_git(['add', 'Packages'])
    
    if run_git(['commit', '-m', "Push chunk: Root files and settings"]):
        run_git(['push', 'origin', 'main'])
    
    # 3. Add and commit Assets subdirectories one by one
    assets_path = 'Assets'
    if os.path.exists(assets_path):
        subitems = os.listdir(assets_path)
        for item in subitems:
            item_path = os.path.join(assets_path, item)
            print(f"Processing {item_path}...")
            run_git(['add', item_path])
            if run_git(['commit', '-m', f"Push chunk: {item_path}"]):
                print(f"Pushing {item_path}...")
                if not run_git(['push', 'origin', 'main']):
                    print(f"Failed to push {item_path}, but commit is saved. Continuing...")
            else:
                print(f"Nothing to commit for {item_path}")

if __name__ == "__main__":
    chunked_push()
