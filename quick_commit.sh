#!/bin/bash
# quick_commit.sh
# Usage: ./quick_commit.sh "Commit message"

# Check for commit message
if [ -z "$1" ]; then
  echo "Error: Commit message required."
  echo "Usage: ./quick_commit.sh \"Your commit message\""
  exit 1
fi

# Stage all changes in Main folder
git add .

# Commit with provided message
git commit -m "$1"

# Push to GitHub
git push
