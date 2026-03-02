#!/bin/bash
# quick_commit.sh
# Usage:
#   ./quick_commit.sh "Commit message"

set -e  # Stop on first error

REPO_URL="https://github.com/brandongrube199928/CSharpCourseWork.git"

# Validate commit message
if [ -z "$1" ]; then
  echo "Error: Commit message required."
  echo "Usage: ./quick_commit.sh \"Commit message\""
  exit 1
fi

COMMIT_MSG="$1"

echo "Using remote: $REPO_URL"

# Initialize repo if not already initialized
if [ ! -d ".git" ]; then
  echo "Initializing new Git repository…"
  git init
  git branch -M main
fi

# Add remote if not already set
if ! git remote get-url origin > /dev/null 2>&1; then
  echo "Adding remote origin…"
  git remote add origin "$REPO_URL"
else
  echo "Remote already configured."
fi

# Stage all changes
git add .

# Only commit if there are staged changes
if git diff --cached --quiet; then
  echo "No changes to commit."
else
  echo "Committing…"
  git commit -m "$COMMIT_MSG"
fi

# Push to GitHub
echo "Pushing to origin/main…"
git push -u origin main

echo "Done!"