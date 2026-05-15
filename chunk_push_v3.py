import os
import subprocess
import sys

def get_all_files(directories):
    all_files = []
    for d in directories:
        if os.path.isfile(d):
            all_files.append(d)
            continue
        for root, dirs, files in os.walk(d):
            for f in files:
                all_files.append(os.path.join(root, f))
    return all_files

def run_git(args):
    # print(f"Running: git {' '.join(args)}")
    result = subprocess.run(['git'] + args, capture_output=True, text=True)
    return result

def chunked_push():
    # Directories/files to process (from git status)
    targets = [
        'Assets/TutorialInfo.meta',
        'Assets/TutorialInfo',
        'Assets/VegetationSpawner.meta',
        'Assets/VegetationSpawner',
        'Assets/sujay.meta',
        'Assets/sujay',
        'Assets/_TerrainAutoUpgrade.meta',
        'Assets/_TerrainAutoUpgrade'
    ]
    
    # Filter out what's actually there
    targets = [t for t in targets if os.path.exists(t)]
    
    all_files = get_all_files(targets)
    print(f"Total files to process: {len(all_files)}")
    
    current_chunk = []
    current_size = 0
    max_chunk_size = 40 * 1024 * 1024 # 40 MB
    
    chunk_count = 0
    
    for f in all_files:
        try:
            size = os.path.getsize(f)
        except OSError:
            continue
            
        current_chunk.append(f)
        current_size += size
        
        if current_size >= max_chunk_size:
            chunk_count += 1
            process_chunk(current_chunk, chunk_count)
            current_chunk = []
            current_size = 0
            
    if current_chunk:
        chunk_count += 1
        process_chunk(current_chunk, chunk_count)

def process_chunk(files, index):
    print(f"Processing chunk {index} ({len(files)} files)...")
    
    # Add files
    for f in files:
        run_git(['add', f])
    
    # Commit
    res = run_git(['commit', '-m', f"Push chunk v3 {index}"])
    if "nothing to commit" in res.stdout or "nothing to commit" in res.stderr:
        print(f"Chunk {index} had nothing to commit.")
        return

    # Push
    print(f"Pushing chunk {index}...")
    retry = 3
    while retry > 0:
        res = run_git(['push', 'origin', 'main'])
        if res.returncode == 0:
            print(f"Chunk {index} pushed successfully.")
            return
        else:
            print(f"Push failed for chunk {index}: {res.stderr}")
            retry -= 1
            if retry > 0:
                print("Retrying...")
    
    print(f"Gave up on chunk {index} after 3 retries.")

if __name__ == "__main__":
    chunked_push()
