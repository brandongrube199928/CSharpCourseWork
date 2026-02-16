#!/bin/bash

# UNIVERSAL Git push script for any project

# Prompt for commit message
read -p "Enter a commit message: " commit_msg

# Prompt for branch name
read -p "Enter a branch name: " branch_name

# Create a basic .gitignore if it doesn't exist
if [ ! -f .gitignore ]; then
cat > .gitignore <<IGNORE
# Build artifacts
bin/
obj/

# IDE files
.vs/
*.user
*.suo

# Test results
*.TestResults/
IGNORE
echo ".gitignore created."
fi

# Initialize Git if necessary
if [ ! -d .git ]; then
    git init
    echo "Git repository initialized."
fi

# Check if remote 'origin' exists
remote_exists=$(git remote | grep origin)
if [ -z "$remote_exists" ]; then
    read -p "Enter GitHub repository URL (HTTPS): " repo_url
    git remote add origin "$repo_url"
    echo "Remote 'origin' added."
fi

# Stage all files
git add .

# Commit changes
git commit -m "$commit_msg"

# Create and switch to the branch
git checkout -b "$branch_name"

# Push to GitHub
git push -u origin "$branch_name"

echo "✅ Branch '$branch_name' pushed to GitHub with commit: $commit_msg"
