# GlideCLI
Glide implements calculations from scientific research on the forgetting curve, to present study material at the right time.


After I made Glide-UWP, I started working on porting the application over to Linux, and was going to build the Linux version of Glide with the ability to build a course, and keep a library of courses that have been built. Then, I was going to add that functionality into the UWP version of Glide, so that a user would not have to touch a single line of code. But I have been wanting to actually use the program for a while, and decided to do it. I realized that making and editing images from a text book of all of the math lessons, problems, and answers, could take weeks. Hardcoding the UWP version of Glide to use the images wouldn't take as long as making them, but it would still take a while, since I hadn't yet built the parts for the users to add a course from the GUI.

I decided that I would rather use flash cards, or just hide the answer of a math problem that is given in a textbookfor me to check when I finish working the problem, than spend weeks doing the work it may have taken to build images of a course for me to study in the program.


This version of Glide does not do what B.F. Skinner's "GLIDER" machine did. That machine presented a question to the respondent, and the correct answer once the respondent is finished producing his or her own answer. "GLIDER" implemented a continuous reinforcement schedule, just like using flash cards would. It also scheduled the spacing of repetitions, but that schedule had no way of being precise. 


GlideCLI implements calculations from research on the forgetting curve, to schedule the spacing of repetitions for the user. GlideCLI also allows the user to add courses into the program very easily. Just prepare your flash cards if you need them, for every section of every chapter, make sure you know which section of which chapter they belong, then start GlideCLI. This program is a lot better than the machine called "GLIDER", as long as you check your answer after every problem, or question.

Research that all calculations are based on, except for two, can be found at https://github.com/Dartomic/GlideCLI/blob/master/5535.pdf

The first calculation that is not based on that paper, are the values and process used to determine "Difficulty." I designed the program to use a Point-Slope formula, and had to find the maximum and minimum output values by comparing the spacing schedules produced, to those used in several experiments.

The other calculation that is not available in that paper, is the amount of time that should exist between the first and second repetitions. Most studies use 24 hours, so that's the amount of time that is used in this software.


This works on Windows, and Linux. It may also work for macOS if you select Linux as your operating system, but I don't have macOS, so I don't know. 
