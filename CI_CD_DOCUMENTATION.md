# CI/CD Documentation

This document explains the Continuous Integration and Continuous Deployment (CI/CD) setup for the VehicleCare backend API.

## Overview

The CI/CD pipeline is implemented using GitHub Actions and consists of several workflows that ensure code quality, security, and reliability.

## Workflows

### 1. Main CI/CD Pipeline (`.github/workflows/ci.yml`)

**Triggers:**
- Push to `main` or `develop` branches
- Pull requests to `main` or `develop` branches

**Jobs:**

#### Build and Test
- ✅ Restores NuGet packages with caching
- ✅ Builds the solution in Release configuration
- ✅ Runs all unit tests (99 tests currently)
- ✅ Collects code coverage data
- ✅ Uploads test results and coverage reports
- ✅ Sends coverage data to Codecov

#### Code Quality Analysis
- ✅ Runs code analysis during build
- ✅ Checks code formatting using `dotnet format`
- ✅ Ensures consistent code style

#### Security Scan
- ✅ Scans for vulnerable NuGet packages
- ✅ Runs GitHub CodeQL analysis for security vulnerabilities
- ✅ Performs static code analysis

#### Dependency Check
- ✅ Checks for outdated packages
- ✅ Identifies security vulnerabilities in dependencies

#### Docker Build Test
- ✅ Tests Docker image build process
- ✅ Runs only on PRs and main branch pushes
- ✅ Uses build cache for efficiency

### 2. Pull Request Validation (`.github/workflows/pr-validation.yml`)

**Triggers:**
- Pull request opened, synchronized, or reopened

**Features:**
- ✅ Fast validation pipeline
- ✅ Posts test results as PR comments
- ✅ Uploads test artifacts
- ✅ Blocks merge if tests fail

## Quality Gates

The following quality gates must pass before code can be merged:

1. **Build Success**: Solution must build without errors
2. **All Tests Pass**: All 99 unit tests must pass
3. **Code Formatting**: Code must follow formatting standards
4. **Security Check**: No known security vulnerabilities
5. **Docker Build**: Docker image must build successfully

## Test Coverage

- **Current Test Count**: 99 tests
- **Coverage Target**: 80%+ (monitored via Codecov)
- **Test Types**:
  - Unit tests for all service classes
  - Validation tests for input validators
  - Authentication and JWT tests
  - Database integration tests

## Local Development

### Running Tests Locally

```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Check code formatting
dotnet format --verify-no-changes
```

### Building Docker Image

```bash
# Build Docker image
docker build -t vehicare-api .

# Run Docker container
docker run -p 8080:80 vehicare-api
```

## Environment Requirements

- **.NET**: 8.0.x
- **Docker**: Latest stable version
- **Dependencies**: Auto-restored via NuGet

## Status Badges

Add these badges to your main README.md:

```markdown
![CI/CD Pipeline](https://github.com/ferdinandosp/vehicare-be/workflows/CI%2FCD%20Pipeline/badge.svg)
![PR Validation](https://github.com/ferdinandosp/vehicare-be/workflows/PR%20Validation/badge.svg)
[![codecov](https://codecov.io/gh/ferdinandosp/vehicare-be/branch/main/graph/badge.svg)](https://codecov.io/gh/ferdinandosp/vehicare-be)
```

## Configuration

### Required Repository Settings

1. **Branch Protection Rules** (Recommended):
   - Require status checks to pass before merging
   - Require branches to be up to date before merging
   - Require review from code owners

2. **Actions Permissions**:
   - Allow GitHub Actions to read and write to repository
   - Allow Actions to comment on pull requests

### Optional Integrations

- **Codecov**: For detailed coverage reports
- **Dependabot**: For automated dependency updates
- **SonarCloud**: For additional code quality metrics

## Troubleshooting

### Common Issues

1. **Test Failures**: Check the test results in the Actions tab
2. **Build Errors**: Verify all dependencies are properly restored
3. **Formatting Issues**: Run `dotnet format` locally before pushing
4. **Docker Build Failures**: Check Dockerfile and .dockerignore configuration

### Debug Commands

```bash
# Restore packages explicitly
dotnet restore

# Build with detailed output
dotnet build --verbosity detailed

# Run specific test
dotnet test --filter "TestName"
```

## Monitoring

- **Test Results**: Available in GitHub Actions artifacts
- **Coverage Reports**: Available on Codecov dashboard
- **Security Alerts**: GitHub Security tab
- **Dependency Alerts**: GitHub Dependabot alerts

## Maintenance

- Review and update dependencies monthly
- Monitor test execution times
- Update GitHub Actions workflows as needed
- Review security scan results regularly
