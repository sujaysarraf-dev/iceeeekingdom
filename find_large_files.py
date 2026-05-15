import os

def get_dir_size(directory):
    total_size = 0
    for root, dirs, files in os.walk(directory):
        if '.git' in dirs:
            dirs.remove('.git')
        for file in files:
            filepath = os.path.join(root, file)
            try:
                total_size += os.path.getsize(filepath)
            except OSError:
                pass
    return total_size

def find_large_files(directory, threshold_mb=50):
    threshold_bytes = threshold_mb * 1024 * 1024
    large_files = []
    for root, dirs, files in os.walk(directory):
        if '.git' in dirs:
            dirs.remove('.git')
        if 'Library' in dirs:
            dirs.remove('Library')
        if 'Temp' in dirs:
            dirs.remove('Temp')
        for file in files:
            filepath = os.path.join(root, file)
            try:
                size = os.path.getsize(filepath)
                if size > threshold_bytes:
                    large_files.append((filepath, size / (1024 * 1024)))
            except OSError:
                pass
    return large_files

if __name__ == "__main__":
    print("--- Large Files (>50MB) ---")
    files = find_large_files(".")
    files.sort(key=lambda x: x[1], reverse=True)
    for f, s in files:
        print(f"{f}: {s:.2f} MB")
    
    print("\n--- Directory Sizes ---")
    for item in os.listdir("."):
        if os.path.isdir(item) and item not in ['.git', 'Library', 'Temp']:
            size = get_dir_size(item)
            print(f"{item}: {size / (1024 * 1024):.2f} MB")
