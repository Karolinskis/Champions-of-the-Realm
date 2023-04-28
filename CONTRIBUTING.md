# Contributing guidelines
Thank you for showing interest in contributing to Champions of the Realm! We welcome all contributions, from bug fixes to new features. Please read the following guidelines before contributing.

## Table of contents


## Reporting bugs
A **bug** is a situatin in which there is something wrong with the game. For example, a feature is not working as intended, or the game crashes.

To tack bug reports, we primarily use GitHub **issues**. To report a bug, please follow these steps:
* Before opening a new issue, please check if there is already an issue for the bug you want to report. If there is, please add a comment to the existing issue instead of opening a new one.
* If there is no existing issue, please open a new issue. Please include as much information as possible, such as:
  * A description of the bug
  * Steps to reproduce the bug
  * Screenshots or videos of the bug
  * Your operating system and version
  * Your game version
  * Any other information you think might be relevant
* We may ask you for more information, so please check back on the issue regularly.

If we cannot reporoduce the bug, we may close the issue. If you think the issue was closed by mistake, please add a comment to the issue.

## Providing general feedback
If you wish to provide subjective feedback on the game (about how the UI looks, about game balance, etc.), suggest a new feature to be added to the game, report a non-specific issue with the game that you think may be connected to your hardware or operating system specifically,

then it's generally best practice to first start with a **discussion**. Discussions are a good way to get feedback from the community and the developers before you start working on a feature or a fix. 

To start a discussion, please follow these steps:
* Check if there is already a discussion about the topic you want to discuss. If there is, please add a comment to the existing discussion instead of starting a new one.
* If proposing a feature, please try to explain the feature as clearly as possible. You can also include mockups or other visual aids to help explain your idea.
* If your'e reporting a non-specific issue, please include as much information as possible, such as:
  * A description of the issue
  * Steps to reproduce the issue
  * Screenshots or videos of the issue
  * Your operating system and version
  * Your game version
  * Any other information you think might be relevant

## Submitting pull requests
We're always happy to accept pull requests! If you want to contribute code to the game. The issue tracker should provide plenty of issues that you can work on.

Here are some key things to keep in mind before jumping in:

* **Make sure you're comfortable with C# and your IDE.** 

  We don't expect you to be an expert, but you should be able to write code that is readable and follows the existing code style.

* **Make sure you're familiar with GitHub and the pull request workflow.**

  [git](https://git-scm.com/) is a distributed version control system that might not be very intuitive to use at first. In particular, projects using git have a particular workflow for submitting code changes, which is called the pull request workflow.

  To make things run more smoothly, we recommend that you look up some online resources to familiarise yourself with the git vocabulary and the pull request workflow. An overview of the process can be found in [this GitHub article](https://docs.github.com/en/github/collaborating-with-issues-and-pull-requests/about-pull-requests).

* **Refrain from making changes through the GitHub web interface.**

  Even though GitHub allows you to make changes to files directly through the web interface, we recommend that you refrain from doing so. Instead, you should clone the repository to your local machine and make changes there. This will make it easier for you to test your changes before submitting them.

  Code written through the web interface will also very likely be questioned outright by the reviewers, as it is likely that it has not ben properly tested. We strongly encourage using an IDE like [Visual Studio Code](https://code.visualstudio.com/).

* **Code analysis**

  Before commiting your code, please run a code formatter. This can be done by running the `dotnet format` command in the root directory of the repository, or using the `Format code` command in your IDE.

* **Add tests for your code whenever possible.**
  
  Automated tests are an essential part of a quality and reliable codebase. They help to make the code more maintainable by ensuring it is safe to reorganise and refactor the code without breaking anything. If it is viable, please put in the time to add tests, so that the changes you make can last for a long time.

* **Run code style analysis before opening a pull request.**

  As part of the pull request workflow one of the things that will be checked is whether your code follows the existing code style, which is supposed to make sure that your code is formatted the same way as all the pre-existing code in the repository. The reason we enforce a particular code style is to make the code more readable and maintainable.

* **Be patient when waiting for the code to be reviewed and merged.**
  
  As much as we'd like to review all contributions as fast as possible, our time is limited. We will try to review your code as soon as possible, but it may take some time. Please be patient and don't spam the issue tracker or the pull request with comments asking for a review.

* **Don't mistake critisim of code for critisim of you.**

  When reviewing code, we may point out things that we think could be improved. This is not meant as a personal attack on you, but rather as a way to help you improve your code. Please don't take it personally.