#!/bin/bash

###############################################################################
# Viking Siege Breaker - Build Automation Script
# Quick setup and build automation for CI/CD and command-line usage
###############################################################################

set -e  # Exit on error

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Unity executable path (modify for your system)
if [[ "$OSTYPE" == "darwin"* ]]; then
    # macOS
    UNITY_PATH="/Applications/Unity/Hub/Editor/6.2.0f1/Unity.app/Contents/MacOS/Unity"
elif [[ "$OSTYPE" == "msys" || "$OSTYPE" == "cygwin" ]]; then
    # Windows
    UNITY_PATH="C:/Program Files/Unity/Hub/Editor/6.2.0f1/Editor/Unity.exe"
else
    # Linux
    UNITY_PATH="/opt/unity/Editor/Unity"
fi

PROJECT_PATH="$(pwd)"
BUILD_OUTPUT="$PROJECT_PATH/Builds"

# Helper functions
print_header() {
    echo -e "${BLUE}================================${NC}"
    echo -e "${BLUE}$1${NC}"
    echo -e "${BLUE}================================${NC}"
}

print_success() {
    echo -e "${GREEN}✓ $1${NC}"
}

print_error() {
    echo -e "${RED}✗ $1${NC}"
}

print_info() {
    echo -e "${YELLOW}ℹ $1${NC}"
}

# Check if Unity is installed
check_unity() {
    if [ ! -f "$UNITY_PATH" ]; then
        print_error "Unity not found at: $UNITY_PATH"
        print_info "Please edit this script and set the correct UNITY_PATH"
        exit 1
    fi
    print_success "Unity found at: $UNITY_PATH"
}

# Build scenes using Unity batch mode
build_scenes() {
    print_header "Building Scenes"

    "$UNITY_PATH" \
        -batchmode \
        -quit \
        -projectPath "$PROJECT_PATH" \
        -executeMethod VikingSiegeBreaker.Editor.QuickBuildTool.QuickBuildAllScenes \
        -logFile "$PROJECT_PATH/build-scenes.log"

    if [ $? -eq 0 ]; then
        print_success "Scenes built successfully!"
    else
        print_error "Scene build failed. Check build-scenes.log"
        exit 1
    fi
}

# Create ScriptableObjects
create_data() {
    print_header "Creating ScriptableObjects"

    "$UNITY_PATH" \
        -batchmode \
        -quit \
        -projectPath "$PROJECT_PATH" \
        -executeMethod VikingSiegeBreaker.Editor.QuickBuildTool.CreateDefaultScriptableObjects \
        -logFile "$PROJECT_PATH/create-data.log"

    if [ $? -eq 0 ]; then
        print_success "ScriptableObjects created successfully!"
    else
        print_error "ScriptableObject creation failed. Check create-data.log"
        exit 1
    fi
}

# Validate project setup
validate_project() {
    print_header "Validating Project Setup"

    "$UNITY_PATH" \
        -batchmode \
        -quit \
        -projectPath "$PROJECT_PATH" \
        -executeMethod VikingSiegeBreaker.Editor.QuickBuildTool.TestBuild \
        -logFile "$PROJECT_PATH/validation.log"

    if [ $? -eq 0 ]; then
        print_success "Validation passed!"
    else
        print_error "Validation failed. Check validation.log"
        exit 1
    fi
}

# Build Android APK
build_android() {
    print_header "Building Android APK"

    mkdir -p "$BUILD_OUTPUT/Android"

    "$UNITY_PATH" \
        -batchmode \
        -quit \
        -projectPath "$PROJECT_PATH" \
        -buildTarget Android \
        -buildPath "$BUILD_OUTPUT/Android/VikingSiegeBreaker.apk" \
        -logFile "$PROJECT_PATH/build-android.log"

    if [ $? -eq 0 ]; then
        print_success "Android build complete!"
        print_info "APK saved to: $BUILD_OUTPUT/Android/VikingSiegeBreaker.apk"
    else
        print_error "Android build failed. Check build-android.log"
        exit 1
    fi
}

# Clean generated files
clean_all() {
    print_header "Cleaning Project"

    "$UNITY_PATH" \
        -batchmode \
        -quit \
        -projectPath "$PROJECT_PATH" \
        -executeMethod VikingSiegeBreaker.Editor.QuickBuildTool.CleanAll \
        -logFile "$PROJECT_PATH/clean.log"

    if [ $? -eq 0 ]; then
        print_success "Project cleaned!"
    else
        print_error "Clean failed. Check clean.log"
        exit 1
    fi
}

# Show help
show_help() {
    echo "Viking Siege Breaker - Build Automation"
    echo ""
    echo "Usage: ./build-automation.sh [command]"
    echo ""
    echo "Commands:"
    echo "  setup          - Complete project setup (scenes + data)"
    echo "  scenes         - Build all scenes only"
    echo "  data           - Create ScriptableObjects only"
    echo "  validate       - Validate project setup"
    echo "  build-android  - Build Android APK"
    echo "  clean          - Delete all generated files"
    echo "  all            - Full setup + validation + build"
    echo "  help           - Show this help message"
    echo ""
    echo "Examples:"
    echo "  ./build-automation.sh setup         # Quick setup"
    echo "  ./build-automation.sh validate      # Check if ready"
    echo "  ./build-automation.sh build-android # Build APK"
    echo "  ./build-automation.sh all           # Complete workflow"
}

# Main script logic
main() {
    print_header "Viking Siege Breaker - Build Automation"

    if [ $# -eq 0 ]; then
        show_help
        exit 0
    fi

    check_unity

    case "$1" in
        setup)
            build_scenes
            create_data
            print_success "Setup complete!"
            ;;
        scenes)
            build_scenes
            ;;
        data)
            create_data
            ;;
        validate)
            validate_project
            ;;
        build-android)
            build_android
            ;;
        clean)
            clean_all
            ;;
        all)
            build_scenes
            create_data
            validate_project
            build_android
            print_success "All tasks complete!"
            ;;
        help|--help|-h)
            show_help
            ;;
        *)
            print_error "Unknown command: $1"
            show_help
            exit 1
            ;;
    esac
}

main "$@"
