/****************************************************************
                        scratchPaper.cs
This file is normally used to design components before
putting them into the program.
****************************************************************/


// I was pretty tired when I wrote a lot of this.
// Go through and correct some of the bad logic.







        /*Start: Create expected date of completing last topic*/

        /***********************************PREDICTION START*********************************************/
        private static void PredictMain()
        {
            // predictVars.Process_Prediction = true; I think I used something else
            // Use actual difficulties for studied topics
            // But get average of real difficulties to use 
            // for non-studied topics for them to be simulated
            AvgDiff();
            
            /*****************/
            // These will be used to plot line of points (x1 , y1) and (x2 , y2)
            // XY correspond to new topics, and repeat topics
            
            

            // Point Ymax is (x1 , y1)
            // For point (x1 , y1), where y1 is maximum first studies performed
            // x1 is number of repeat studies performed at max first studies
            CollectFirstStudies();      // Get first study dates of studied topics
            if (studiedSimList.Count > Constants.ZERO_INT)
            {
                YmaxFirsts();               // Get y1: max Y First Studies for point Ymax
                SimulatePastStudies();      // Produce real repeats
                YmaxRepeats();              // Get x1: max X Repeat Studies for point Ymax

                // Since all the past study sessions are simulated and collected 
                // already, we just need to get the XmaxRepeats and XmaxFirsts.

                // Point Xmax (x2 , y2)
                // For point (x2 , y2), where x2 is maximum  repeat studies performed
                // y2 is number of first studies performed at max repeat studies
                XmaxRepeats();
                XmaxFirsts();

                //predictVars.Loop_Entered = false; // Gets set to true if completion date not studied by the user yet. 
                GenerateProjectedStudies();
                /*****************/

                // Make function to clear lists that need to be cleared.
                // Do not call it here.
                // Call it after each real repetition is performed
                // This will allow results to update in real-time

                // predictVars.Process_Prediction = false; I think I used something else
            }
        }


        private static void AvgDiff()
        {
            // Apply average of difficulty to simulation of 
            // calculating non-studied topics learning dates.
            double nDifficultsDouble = Constants.ZERO_DOUBLE;
            double difficultsAdded = Constants.ZERO_DOUBLE;
            predictVars.Avg_Difficulty = Constants.ZERO_DOUBLE;
            int count = Constants.ZERO_INT;
            foreach (var topic in TopicsList)
            {
                if (topic.Top_Studied == true)
                {
                    difficultsAdded += Convert.ToDouble(topic.Top_Difficulty);
                    ++count;
                }
            }
            nDifficultsDouble = Convert.ToDouble(count);
            predictVars.Avg_Difficulty = difficultsAdded/nDifficultsDouble;
        }
        private static void CollectFirstStudies()
        {
            SimModel newSims = new SimModel();
            int topicNumber = Constants.ZERO_INT;
            foreach (var topic in TopicsList)
                if (topic.Top_Studied == true)
                {
                    newSims.First_Date = topic.First_Date;
                    newSims.Real_Repetition = topic.Top_Repetition;
                    newSims.Sim_Repetition = Constants.ZERO_INT;
                    newSims.Top_Difficulty = topic.Top_Difficulty;
                    newSims.Interval_Length = topic.Interval_Length;
                    newSims.Top_Number = topicNumber; 
                    studiedSimList.Add(newSims);
                    ++topicNumber;
                }
        }
        private static void YmaxFirsts()
        {
            List<string> fStudyDates = new List<string>();
            List<int> fStudyCounts = new List<int>();
            bool firstCheck = true;
            int index = Constants.ZERO_INT;
            int count = Constants.ONE_INT;
            int dateCompare;
            DateTime tempDateOne;
            DateTime tempDateTwo;    


            foreach (var topic in studiedSimList)
            {
                if (firstCheck == true)
                {
                    fStudyDates.Add($"{topic.First_Date}");
                    fStudyCounts.Add(count);
                    firstCheck = false;
                }
                else
                {
                    tempDateOne = DateTime.Parse(fStudyDates[index]);
                    tempDateTwo = DateTime.Parse(topic.First_Date);
                    dateCompare = DateTime.Compare(tempDateOne, tempDateTwo);

                    if (dateCompare == Constants.ZERO_INT)
                        ++fStudyCounts[index];
                    else
                    {
                        ++index;
                        fStudyDates.Add($"{topic.First_Date}");
                        fStudyCounts.Add(count);
                    }            
                }        
            }
            
            /******************
            Sorting Section Start
            ******************/
            int j, i, keyOne;
            string keyTwo;

            i = Constants.ZERO_INT;
            j = Constants.ONE_INT;
            keyOne = Constants.ZERO_INT;
            count = Constants.ZERO_INT;
            while (count < Constants.TWO_INT)
            {  
                
                for (j = Constants.TWO_INT; j < fStudyCounts.Count; ++j)
                {
                    keyOne = fStudyCounts[j];
                    keyTwo = fStudyDates[j];

                    // Insert fStudyCounts[j] into sorted sequence fStudyCounts[1...j-1]
                    // Insert fStudyDates[j] into sorted sequence fStudyDates[1...j-1]
                    i = j - Constants.ONE_INT;
                    while (i > Constants.ONE_INT && fStudyCounts[i] > keyOne)
                    {
                        fStudyCounts[i + Constants.ONE_INT] = fStudyCounts[i];
                        fStudyDates[i + Constants.ONE_INT] = fStudyDates[i];
                        i = i - Constants.ONE_INT;
                    }
                    fStudyCounts[i + Constants.ONE_INT] = keyOne;
                    fStudyDates[i + Constants.ONE_INT] = keyTwo;
                }

                /* 
                this is here to get the first element sorted into 
                the rest of the array on the second run of the loop
                */
                if (count == Constants.ZERO_INT)
                {
                    //key = A[ZERO];
                    keyOne = fStudyCounts[Constants.ZERO_INT];
                    keyTwo = fStudyDates[Constants.ZERO_INT];
                    //A[ZERO] = A[ONE];
                    fStudyCounts[Constants.ZERO_INT] = fStudyCounts[Constants.ONE_INT];
                    fStudyDates[Constants.ZERO_INT] = fStudyDates[Constants.ONE_INT];
                    //A[ONE] = key;
                    fStudyCounts[Constants.ONE_INT] = keyOne;
                    fStudyDates[Constants.ONE_INT] = keyTwo;
                }
                ++count;
            }
            /******************
            Sorting Section End
            ******************/


            // Each of the dates with the equally highest number of 
            // first topics studied needs to be used, so that the 
            // one with the highest number of repetitions also performed 
            // can be used for Y_High_Xcount Since this method does not 
            // count previously studied repetitions, then the dates just 
            // need to be passed to a list instead of the first single 
            // date with the highest Y count being passed to a variable.
            
            List<int> elementList = new List<int>();
            int check = Constants.ZERO_INT;
            index = fStudyCounts.Count - Constants.ONE_INT;
            firstCheck = true;
            while (index > Constants.ZERO_INT)
            {
                if (firstCheck == true)
                {
                    firstCheck = false;
                    elementList.Add(index);
                    check = fStudyCounts.ElementAt(index);
                }
                else if ((index - Constants.ONE_INT) >= Constants.ZERO_INT)
                    if (check == fStudyCounts.ElementAt(index - Constants.ONE_INT))
                        elementList.Add(index - Constants.ONE_INT);
                --index;
            }

            // Add all highest dates to list. Date with highest X-value
            // will be selected from the yMaxList later.
            //List<PointLimits> tempElements = new List<PointLimits>();
            
            index = Constants.ZERO_INT;
            while (index < elementList.Count)
            {
                PointLimits tempElements = new PointLimits();
                tempElements.High_Date = fStudyDates[elementList[index]];
                tempElements.Y_Count = fStudyCounts[elementList[index]];
                
                yMaxList.Add(tempElements);
                ++index;
            }
        }
        private static void SimulatePastStudies()
        {
            SimModel studiedSims = new SimModel();
            bool firstRep = true;
            // DELETEME remove this later if I didn't need it: string repetitionDate = " ";
            int repetitionIndex = Constants.ZERO_INT;
            predictVars.Gen_Studied_Index = Constants.ZERO_INT;

            foreach (var topic in studiedSimList)
            {
                while (repetitionIndex < topic.Real_Repetition)
                {
                    studiedSims.First_Date = topic.First_Date;
                    studiedSims.Real_Repetition = topic.Real_Repetition;
                    if (firstRep == true)
                    {
                        studiedSims.Repetition_Date = topic.First_Date;
                        studiedSims.Sim_Repetition = Constants.ZERO_INT;
                        firstRep = false;
                    }
                    else
                    {
                        studiedSims.Repetition_Date = genSimsStudied.ElementAt(predictVars.Gen_Studied_Index - Constants.ONE_INT).Next_Date;
                    }
                    studiedSims.Top_Difficulty = topic.Top_Difficulty;
                    studiedSims.Top_Number = topic.Top_Number; 
                    genSimsStudied.Add(studiedSims);

                    predictVars.Process_Gen_Sims_Studied = true;
                    SimCalculateLearning();
                    predictVars.Process_Gen_Sims_Studied = false;

                    ++predictVars.Gen_Studied_Index;
                    repetitionIndex = studiedSims.Sim_Repetition;
                }
                repetitionIndex = Constants.ZERO_INT;
                firstRep = true;
            }
            predictVars.Gen_Studied_Index = Constants.ZERO_INT;
        }
        private static void YmaxRepeats()
        {
            // Use the date of highest first studies, 
            // with the highest number of repetitions occuring on that first date
            // check all non-first repetions in a loop
            // If date == Y_High_Date, Then Add 1 to Y_High_Xcount
            // Do this until there are no more elements
            DateTime topicDate;
            DateTime yMaxDate;      // There can exist more than one date with same Y-value
            int dateCompare;
            int index = Constants.ZERO_INT;

            while (index < yMaxList.Count)
            {
                yMaxDate = DateTime.Parse(yMaxList.ElementAt(index).High_Date);
                foreach (var topic in genSimsStudied)
                {
                    topicDate =  DateTime.Parse(topic.Repetition_Date);
                    dateCompare = DateTime.Compare(topicDate, yMaxDate);
                    if (topic.Sim_Repetition > Constants.ONE_INT && dateCompare == Constants.ZERO_INT)
                        ++yMaxList.ElementAt(index).X_Count;
                }
                ++index;
            }

            double check = Constants.ZERO_INT;
            int useable = Constants.ZERO_INT;
            index = Constants.ZERO_INT;
            check = yMaxList.ElementAt(index).X_Count;
            while (index < yMaxList.Count)
            {
                if ((index + Constants.ONE_INT) < yMaxList.Count)
                    if (check < yMaxList.ElementAt(index + Constants.ONE_INT).X_Count)
                    {
                        check = yMaxList.ElementAt(index + Constants.ONE_INT).X_Count;
                        useable = index + Constants.ONE_INT;
                    }
                ++index;
            }
            predictVars.Y_High_Ycount = yMaxList.ElementAt(useable).Y_Count;
            predictVars.Y_High_Xcount = yMaxList.ElementAt(useable).X_Count;
        }
        private static void XmaxRepeats()
        {
            // I have to make a sorted copy of genSimsStudied
            List<SimModel> studiedSims = new List<SimModel>();
            foreach (var sim in genSimsStudied)
            {
                xMaxSortList.Add(sim);
            }
            XmaxRepeatSort();
            foreach (var sim in xMaxSortList)
            {
                studiedSims.Add(sim);
            }
            xMaxSortList.Clear();

            //Get X
            List<string> tempDates = new List<string>();
            List<int> dateCounts = new List<int>();
            int index = Constants.ZERO_INT;
            int firstRep = Constants.ONE_INT;
            bool firstCheck = true;

            // int32 something = DateTime.Compare(t1 , t2);
            // if something < zero, then t1 is earlier than t2
            // if something == zero, then same day
            // if something > zero, then t1 is later than t2
            foreach (var section in studiedSims)
            {
                if (firstCheck == true)
                {
                    if (firstRep != section.Sim_Repetition)
                    {
                        dateCounts.Add(Constants.ZERO_INT);
                        tempDates.Add(section.Repetition_Date);
                        firstCheck = false;
                    }
                }
                if (firstCheck == false) // Must not be an else for logic to work
                {
                    if (firstRep != section.Sim_Repetition)
                    {
                        if (tempDates.ElementAt(index) == section.Repetition_Date)
                            ++dateCounts[index];
                        else
                        {
                            ++index;
                            tempDates.Add(section.Repetition_Date);
                            dateCounts.Add(Constants.ONE_INT);
                        }
                    }
                }
            }

            // Each of the dates with the equally highest number of 
            // repeat topics studied needs to be used, so that the 
            // one with the highest number of first studies also performed 
            // can be used for X_High_Ycount Since this method does not 
            // count first study repetitions, then the dates just 
            // need to be passed to a list instead of the first single 
            // date with the highest X count being passed to a variable.
            
            List<int> elementList = new List<int>();
            int check = Constants.ZERO_INT;
            index = dateCounts.Count - Constants.ONE_INT;
            firstCheck = true;
            while (index > Constants.ZERO_INT)
            {
                if (firstCheck == true)
                {
                    firstCheck = false;
                    elementList.Add(index);
                    check = dateCounts.ElementAt(index);
                }
                else if ((index - Constants.ONE_INT) >= Constants.ZERO_INT)
                    if (check == dateCounts.ElementAt(index - Constants.ONE_INT))
                        elementList.Add(index - Constants.ONE_INT);
                --index;
            }

            // Add all highest dates to list. Date with highest Y-intercept
            // will be selected from the xMaxList later.
            PointLimits tempElements = new PointLimits();
            index = Constants.ZERO_INT;
            while (index < elementList.Count)
            {
                tempElements.High_Date = tempDates.ElementAt(elementList[index]);
                tempElements.X_Count = dateCounts.ElementAt(elementList[index]);
                
                xMaxList.Add(tempElements);
                ++index;
            }
            
        }
        private static void XmaxRepeatSort()
        {
            SimModel listKey = new SimModel();
            int j, i, dateCheck, count;

            i = Constants.ZERO_INT;
            j = Constants.ONE_INT;
            count = Constants.ZERO_INT;
            while (count < Constants.TWO_INT)
            {  
                
                for (j = Constants.TWO_INT; j < xMaxSortList.Count; ++j)
                {
                    listKey = xMaxSortList.ElementAt(j);

                    // Insert xMaxSortList[j] into sorted sequence xMaxSortList[1...j-1]
                    i = j - Constants.ONE_INT;
                    dateCheck = DateTime.Compare(DateTime.Parse(xMaxSortList.ElementAt(i).Repetition_Date), DateTime.Parse(listKey.Repetition_Date));
                    while (i > Constants.ONE_INT && dateCheck > Constants.ZERO_INT)
                    {
                        xMaxSortList[i + Constants.ONE_INT] = xMaxSortList[i];
                        i = i - Constants.ONE_INT;
                    }
                    xMaxSortList[i + Constants.ONE_INT] = listKey;
                }

                /* 
                this is here to get the first element sorted into 
                the rest of the array on the second run of the loop
                */
                if (count == Constants.ZERO_INT)
                {
                    //key = A[ZERO];
                    listKey = xMaxSortList[Constants.ZERO_INT];
                    //A[ZERO] = A[ONE];
                    xMaxSortList[Constants.ZERO_INT] = xMaxSortList[Constants.ONE_INT];
                    //A[ONE] = key;
                    xMaxSortList[Constants.ONE_INT] = listKey;
                }
                ++count;
            }
        }
        private static void XmaxFirsts()
        {
            // Use the date of highest repetition studies, 
            // with the highest number of first studies occuring on that repetition date
            // check all repetions in a loop
            // If date == X_High_Date, && Sim_Repetition == 1
            // Then Add 1 to X_High_Ycount
            // Do this until there are no more elements
            DateTime topicDate;
            DateTime xMaxDate;      // There can exist more than one date with same X-value
            int dateCompare;
            int index = Constants.ZERO_INT;

            while (index < xMaxList.Count)
            {
                xMaxDate = DateTime.Parse(xMaxList.ElementAt(index).High_Date);
                foreach (var topic in genSimsStudied)
                {
                    topicDate =  DateTime.Parse(topic.Repetition_Date);
                    dateCompare = DateTime.Compare(topicDate, xMaxDate);
                    if (topic.Sim_Repetition == Constants.ONE_INT && dateCompare == Constants.ZERO_INT)
                        ++xMaxList.ElementAt(index).Y_Count;
                }
                ++index;
            }

            double check = Constants.ZERO_INT;
            int useable = Constants.ZERO_INT;
            index = Constants.ZERO_INT;
            check = xMaxList.ElementAt(index).Y_Count;
            while (index < xMaxList.Count)
            {
                if ((index + Constants.ONE_INT) < xMaxList.Count)
                    if (check < xMaxList.ElementAt(index + Constants.ONE_INT).Y_Count)
                    {
                        check = xMaxList.ElementAt(index + Constants.ONE_INT).Y_Count;
                        useable = index + Constants.ONE_INT;
                    }
                ++index;
            }
            predictVars.X_High_Ycount = xMaxList.ElementAt(useable).Y_Count;
            predictVars.X_High_Xcount = xMaxList.ElementAt(useable).X_Count;
        }

        private static void CollectNonStudied()
        {
            SimModel newSims = new SimModel();
            int index = studiedSimList.Count - Constants.ONE_INT;
            int topicNumber = studiedSimList.ElementAt(index).Top_Number + Constants.ONE_INT;
            foreach (var topic in TopicsList)
                if (topic.Top_Studied == false)
                {
                    newSims.First_Date = topic.First_Date;
                    newSims.Real_Repetition = topic.Top_Repetition;
                    newSims.Sim_Repetition = Constants.ZERO_INT;
                    newSims.Top_Difficulty = predictVars.Avg_Difficulty;
                    newSims.Interval_Length = Constants.ZERO_INT;
                    newSims.Top_Number = topicNumber; 
                    projectedSimList.Add(newSims);
                    ++topicNumber;
                }
        }




        private static void GenerateProjectedStudies()
        {
            DateTime startDate, previousDate, simDate;
            int totalNewTopics, count, predictedIndex;
            startDate = DateTime.Now;
            predictVars.Sim_Date_Use = startDate.ToString("d");
            totalNewTopics = projectedSimList.Count;
            count = Constants.ZERO_INT;

            predictVars.Process_Gen_Sims_Studied = false;
            while (count < totalNewTopics)
            {
                // if (TopicsList.ElementAt(Constants.ZERO_INT).Top_Studied == true)
                //     predictVars.Loop_Entered = true; // Can't predict a date to display if there is no data to work with yet
                PredictStudies();
                previousDate = DateTime.Parse(predictVars.Sim_Date_Use);
                simDate = previousDate.AddDays(Constants.ONE_INT);
                predictVars.Sim_Date_Use = simDate.ToString("d");
                totalNewTopics = projectedSimList.Count;
            }
            Console.WriteLine("DELETEME: Inside GenerateProjectedStudies\nGetting studiedSimList last date.\n");
            Console.ReadLine();
            predictedIndex = studiedSimList.Count - Constants.ONE_INT;
            predictVars.Prediction_Date = studiedSimList.ElementAt(predictedIndex).Next_Date;
            studiedSimList.Clear();
            genSimsStudied.Clear();
            projectedSimList.Clear();
            predictVars.Process_Gen_Sims_Studied = true;
        }
        private static void PredictStudies()
        {
            // Index values for elements to be calculated 
            // stored in studyRepElements list.
            int totalNewTopics = Constants.ZERO_INT;
            int predictedIndex = Constants.ZERO_INT;
            CollectStudyX();
            FindYatX();
            CollectStudyY();
            ReduceNew();

            int index = Constants.ZERO_INT;
            while (index < studyRepElements.Count)
            {
                predictVars.Gen_Projected_Index = studyRepElements.ElementAt(index);
                SimCalculateLearning();
                ++index;
            }
            studyRepElements.Clear();
            totalNewTopics = projectedSimList.Count;
            Console.WriteLine("DELETEME: Inside PredictStudies\nGetting gensimsAll last date.\n");
            Console.ReadLine();
            if (totalNewTopics == Constants.ZERO_INT)
            {
                predictedIndex = genSimsAll.Count - Constants.ONE_INT;
                predictVars.Prediction_Date = genSimsAll.ElementAt(predictedIndex).Next_Date;
            }
            genSimsAll.Clear();
        }
        private static void CollectStudyX()
        {
            
            DateTime useDate, nextDate;
            int dateCompare;
            useDate = DateTime.Parse(predictVars.Sim_Date_Use);

            // Get 2nd rep studies
            int index, repCheck;
            index = repCheck = Constants.ZERO_INT;
            foreach (var topic in studiedSimList)
            {
                nextDate = DateTime.Parse(topic.Next_Date);
                dateCompare = DateTime.Compare(nextDate, useDate);

                if (dateCompare <= Constants.ZERO_INT && topic.Real_Repetition == Constants.TWO_INT)
                    if (repCheck < predictVars.X_High_Xcount)
                    {
                        studyRepElements.Add(index);
                        ++repCheck;
                    }
                ++index;
            }

            // Get Late
            index = Constants.ZERO_INT;
            foreach (var topic in studiedSimList)
            {
                nextDate = DateTime.Parse(topic.Next_Date);
                dateCompare = DateTime.Compare(nextDate, useDate);

                if (dateCompare < Constants.ZERO_INT && topic.Real_Repetition != Constants.TWO_INT)
                    if (repCheck < predictVars.X_High_Xcount)
                    {
                        studyRepElements.Add(index);
                        ++repCheck;
                    }
                ++index;
            }
            
            //Get On-Time
            index = Constants.ZERO_INT;
            foreach (var topic in studiedSimList)
            {
                nextDate = DateTime.Parse(topic.Next_Date);
                dateCompare = DateTime.Compare(nextDate, useDate);

                if (dateCompare == Constants.ZERO_INT && topic.Real_Repetition != Constants.TWO_INT)
                    if (repCheck < predictVars.X_High_Xcount)
                    {
                        studyRepElements.Add(index);
                        ++repCheck;
                    }
                ++index;
            }
            predictVars.Current_X = repCheck;
            foreach (var sim in studiedSimList)
            {
                genSimsAll.Add(sim);
            }
        }
        private static void FindYatX()
        {
            double xOne, yOne, xTwo, yTwo, slopeM, yCurrent, xCurrent, valueB;
            xOne = predictVars.Y_High_Xcount;
            yOne = predictVars.Y_High_Ycount;
            xTwo = predictVars.X_High_Xcount;
            yTwo = predictVars.X_High_Ycount;
            xCurrent = predictVars.Current_X;
            valueB = yOne;
            slopeM = (yTwo - yOne) / (xTwo - xOne);
            yCurrent = (slopeM * xCurrent) + valueB;

            predictVars.Current_Y = (int)yCurrent;
        }
        private static void CollectStudyY()
        {
            int yToStudied = genSimsAll.Count - Constants.ONE_INT;
            int index = Constants.ZERO_INT;
            while (index < predictVars.Current_Y)
            {
                if (projectedSimList.Count >= predictVars.Current_Y)
                {
                    ++yToStudied;
                    studyRepElements.Add(yToStudied);
                }
                ++index;
            }
            index = Constants.ZERO_INT;
            while (index < predictVars.Current_Y)
            {
                if (projectedSimList.Count >= predictVars.Current_Y)
                {
                    genSimsAll.Add(projectedSimList.ElementAt(index));
                    ++index;
                }
                else if (projectedSimList.Count > Constants.ZERO_INT)
                    foreach (var sim in projectedSimList)
                    {
                        genSimsAll.Add(sim);
                    }
            }
        }
        private static void ReduceNew()
        {
            List<SimModel> reducedProjected = new List<SimModel>();
            int index = Constants.ZERO_INT;
            if (projectedSimList.Count > Constants.ZERO_INT)
            {
                foreach (var sim in projectedSimList)
                {
                    if (index >= predictVars.Current_Y)
                        reducedProjected.Add(sim);
                    ++index;
                }
                projectedSimList.Clear();
                foreach (var sim in reducedProjected)
                {
                    projectedSimList.Add(sim);
                }
            }
        }



        /**************SimulateDates**************/
        private static void SimCalculateLearning()
        {
            SimAddRepetition();
            SimIntervalTime();
            SimProcessDate();
        }
        private static void SimAddRepetition()
        {
            if (predictVars.Process_Gen_Sims_Studied == true)
                ++genSimsStudied.ElementAt(predictVars.Gen_Studied_Index).Sim_Repetition;
            else
                ++genSimsAll.ElementAt(predictVars.Gen_Projected_Index).Sim_Repetition;
        }
        private static void SimIntervalTime()
        {
            const double SINGLE_DAY = 1440; // 1440 is the quatity in minutes of a day. I'm using minutes, instead of whole days, to be more precise.
            double difficulty;
            int ithRepetition;
            double intervalLength;

            if (predictVars.Process_Gen_Sims_Studied == true)
            {
                difficulty = genSimsStudied.ElementAt(predictVars.Gen_Studied_Index).Top_Difficulty;
                ithRepetition = genSimsStudied.ElementAt(predictVars.Gen_Studied_Index).Sim_Repetition;
                intervalLength = genSimsStudied.ElementAt(predictVars.Gen_Studied_Index).Interval_Length;
            }
            else
            {
                difficulty = genSimsAll.ElementAt(predictVars.Gen_Projected_Index).Top_Difficulty;
                ithRepetition = genSimsAll.ElementAt(predictVars.Gen_Projected_Index).Sim_Repetition;
                intervalLength = genSimsAll.ElementAt(predictVars.Gen_Projected_Index).Interval_Length;
            }


            //     Second repetition will occur the next day. 
            //	   Although, the research document does not state
            //	   a time frame until the second repetition. The 
            //	   values of the two variables may need to be changed, 
            //	   if the spacing is too far apart. So far they seem fine.
            if (ithRepetition == Constants.ONE_INT)
            {
                intervalLength = SINGLE_DAY;
            }
            else
                intervalLength = intervalLength * difficulty;
            if (predictVars.Process_Gen_Sims_Studied == true)
                genSimsStudied.ElementAt(predictVars.Gen_Studied_Index).Interval_Length = intervalLength;
            else
                genSimsAll.ElementAt(predictVars.Gen_Projected_Index).Interval_Length = intervalLength;
        }
        private static void SimProcessDate()
        {
            const double SINGLE_DAY = 1440;
            double intervalLength;
            double days;
            DateTime fakeToday;
            DateTime nextDate;
            string nextDateString;

            if (predictVars.Process_Gen_Sims_Studied == true)
            {
                intervalLength = genSimsStudied.ElementAt(predictVars.Gen_Studied_Index).Interval_Length;
                days = Convert.ToInt32(intervalLength / SINGLE_DAY);
                fakeToday = DateTime.Parse(genSimsStudied.ElementAt(predictVars.Gen_Studied_Index).Repetition_Date);
                nextDate = fakeToday.AddDays(days);
                nextDateString = nextDate.ToString("d");
            }
            else
            {
                intervalLength = genSimsAll.ElementAt(predictVars.Gen_Projected_Index).Interval_Length;
                days = Convert.ToInt32(intervalLength / SINGLE_DAY);
                fakeToday = DateTime.Parse(genSimsAll.ElementAt(predictVars.Gen_Projected_Index).Repetition_Date);
                nextDate = fakeToday.AddDays(days);
                nextDateString = nextDate.ToString("d");
            }
            
            if (predictVars.Process_Gen_Sims_Studied == true)
                genSimsStudied.ElementAt(predictVars.Gen_Studied_Index).Next_Date = nextDateString;
            else
                genSimsAll.ElementAt(predictVars.Gen_Projected_Index).Next_Date = nextDateString;
        }
        /***********************************PREDICTION END*********************************************/
        /*End: Create expected date of completing last topic*/
