#!/usr/bin/env python3
"""
Viking Siege Breaker - Scene Builder CLI
Cross-platform automation tool for quick project setup
"""

import os
import sys
import subprocess
import platform
import argparse
from pathlib import Path

# ANSI color codes
class Colors:
    HEADER = '\033[95m'
    BLUE = '\033[94m'
    GREEN = '\033[92m'
    YELLOW = '\033[93m'
    RED = '\033[91m'
    END = '\033[0m'
    BOLD = '\033[1m'

def print_header(text):
    print(f"\n{Colors.BLUE}{Colors.BOLD}{'='*60}{Colors.END}")
    print(f"{Colors.BLUE}{Colors.BOLD}{text:^60}{Colors.END}")
    print(f"{Colors.BLUE}{Colors.BOLD}{'='*60}{Colors.END}\n")

def print_success(text):
    print(f"{Colors.GREEN}✓ {text}{Colors.END}")

def print_error(text):
    print(f"{Colors.RED}✗ {text}{Colors.END}")

def print_info(text):
    print(f"{Colors.YELLOW}ℹ {text}{Colors.END}")

def get_unity_path():
    """Get Unity executable path based on platform"""
    system = platform.system()

    if system == "Darwin":  # macOS
        return "/Applications/Unity/Hub/Editor/6.2.0f1/Unity.app/Contents/MacOS/Unity"
    elif system == "Windows":
        return "C:/Program Files/Unity/Hub/Editor/6.2.0f1/Editor/Unity.exe"
    else:  # Linux
        return "/opt/unity/Editor/Unity"

def check_unity(unity_path):
    """Check if Unity executable exists"""
    if not os.path.exists(unity_path):
        print_error(f"Unity not found at: {unity_path}")
        print_info("Please install Unity 6.2+ or edit this script to set the correct path")
        return False
    print_success(f"Unity found at: {unity_path}")
    return True

def run_unity_method(unity_path, project_path, method_name, log_file):
    """Execute a Unity editor method in batch mode"""
    cmd = [
        unity_path,
        "-batchmode",
        "-quit",
        "-projectPath", project_path,
        "-executeMethod", method_name,
        "-logFile", log_file
    ]

    print_info(f"Running: {method_name}")

    try:
        result = subprocess.run(cmd, check=True, capture_output=True, text=True)
        return True
    except subprocess.CalledProcessError as e:
        print_error(f"Failed! Check {log_file} for details")
        return False

def build_scenes(unity_path, project_path):
    """Build all scenes"""
    print_header("Building Scenes")
    log_file = os.path.join(project_path, "build-scenes.log")

    if run_unity_method(
        unity_path,
        project_path,
        "VikingSiegeBreaker.Editor.QuickBuildTool.QuickBuildAllScenes",
        log_file
    ):
        print_success("Scenes built successfully!")
        return True
    return False

def create_data(unity_path, project_path):
    """Create ScriptableObjects"""
    print_header("Creating ScriptableObjects")
    log_file = os.path.join(project_path, "create-data.log")

    if run_unity_method(
        unity_path,
        project_path,
        "VikingSiegeBreaker.Editor.QuickBuildTool.CreateDefaultScriptableObjects",
        log_file
    ):
        print_success("ScriptableObjects created successfully!")
        return True
    return False

def validate_project(unity_path, project_path):
    """Validate project setup"""
    print_header("Validating Project Setup")
    log_file = os.path.join(project_path, "validation.log")

    if run_unity_method(
        unity_path,
        project_path,
        "VikingSiegeBreaker.Editor.QuickBuildTool.TestBuild",
        log_file
    ):
        print_success("Validation passed!")
        return True
    return False

def quick_setup():
    """Interactive setup wizard"""
    print_header("Viking Siege Breaker - Quick Setup Wizard")

    project_path = os.getcwd()
    unity_path = get_unity_path()

    print_info(f"Project: {project_path}")
    print_info(f"Unity: {unity_path}")

    if not check_unity(unity_path):
        return False

    print("\nWhat would you like to do?\n")
    print("1. Quick Setup (Build scenes + Create data)")
    print("2. Build scenes only")
    print("3. Create ScriptableObjects only")
    print("4. Validate project")
    print("5. Full workflow (Setup + Validate)")
    print("0. Exit")

    choice = input("\nEnter choice (1-5): ").strip()

    success = True

    if choice == "1":
        success = build_scenes(unity_path, project_path) and create_data(unity_path, project_path)
    elif choice == "2":
        success = build_scenes(unity_path, project_path)
    elif choice == "3":
        success = create_data(unity_path, project_path)
    elif choice == "4":
        success = validate_project(unity_path, project_path)
    elif choice == "5":
        success = (
            build_scenes(unity_path, project_path) and
            create_data(unity_path, project_path) and
            validate_project(unity_path, project_path)
        )
    elif choice == "0":
        print_info("Goodbye!")
        return True
    else:
        print_error("Invalid choice")
        return False

    if success:
        print_header("✓ Complete!")
        print_info("Your project is ready for development!")
        print_info("Open Unity and start building your game!")
    else:
        print_header("✗ Failed")
        print_info("Check the log files for details")

    return success

def main():
    parser = argparse.ArgumentParser(
        description="Viking Siege Breaker - Scene Builder CLI",
        formatter_class=argparse.RawDescriptionHelpFormatter,
        epilog="""
Examples:
  python scene-builder.py                  # Interactive mode
  python scene-builder.py --setup          # Quick setup
  python scene-builder.py --scenes         # Build scenes only
  python scene-builder.py --data           # Create data only
  python scene-builder.py --validate       # Validate project
  python scene-builder.py --all            # Full workflow
        """
    )

    parser.add_argument("--setup", action="store_true", help="Build scenes and create data")
    parser.add_argument("--scenes", action="store_true", help="Build scenes only")
    parser.add_argument("--data", action="store_true", help="Create ScriptableObjects only")
    parser.add_argument("--validate", action="store_true", help="Validate project setup")
    parser.add_argument("--all", action="store_true", help="Full workflow (setup + validate)")
    parser.add_argument("--unity-path", type=str, help="Custom Unity executable path")

    args = parser.parse_args()

    project_path = os.getcwd()
    unity_path = args.unity_path if args.unity_path else get_unity_path()

    # Interactive mode if no arguments
    if not any(vars(args).values()):
        return quick_setup()

    # Check Unity
    if not check_unity(unity_path):
        return False

    success = True

    # Execute requested operations
    if args.scenes or args.setup or args.all:
        success = success and build_scenes(unity_path, project_path)

    if args.data or args.setup or args.all:
        success = success and create_data(unity_path, project_path)

    if args.validate or args.all:
        success = success and validate_project(unity_path, project_path)

    if success:
        print_header("✓ Complete!")
    else:
        print_header("✗ Some operations failed")
        sys.exit(1)

if __name__ == "__main__":
    try:
        main()
    except KeyboardInterrupt:
        print("\n\nInterrupted by user")
        sys.exit(1)
    except Exception as e:
        print_error(f"Unexpected error: {e}")
        sys.exit(1)
