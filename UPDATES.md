Smart-X — Lecturer Feedback and Updates

Purpose

This document records the updates made to the Smart-X application following the feedback received during the earlier stages of the project.

Feedback from Part 1

The Part 1 submission received a mark of 91%. Feedback was limited because the submission largely met the required standard successfully.

No major corrective changes were identified as necessary from the available lecturer feedback.

Updates Implemented for Part 2

Although there were no major corrective issues to address, the application was extended according to the Part 2 requirements of the PoE.

1. Real-Time Command Stream and History

A new Real-Time Command Stream and History section was implemented, allowing operators to:

View registered devices.

Search and filter devices.

Execute manual device commands.

View command history.

Undo the most recent command.

2. Queue-Based Telemetry Processing

A Queue<T> was implemented for normal telemetry processing using FIFO behaviour.

3. Priority-Based Critical Alerts

A PriorityQueue was implemented to separate critical telemetry from normal telemetry and allow critical alerts to be processed with priority.

4. Stack-Based Command History

A Stack<T> was implemented for manual command history. This supports the Undo functionality by reverting the most recently executed command.

5. Dictionary-Based Device Management

A Dictionary<TKey,TValue> was introduced for live registered-device management. The unique sensor/device identifier is used as the lookup key to provide efficient device retrieval.

6. Sorted Historical Telemetry

A SortedDictionary<TKey,TValue> was implemented to organise historical telemetry by timestamp.

7. HashSet Error Tracking

A HashSet<T> was implemented to maintain unique active device error/disconnection states and prevent duplicate error entries.

8. Search History

Search activity is recorded so that repeated operator searches can be analysed by the recommendation engine.

9. Predictive Suggested Actions

A recommendation engine was added to analyse:

Repeated searches.

Repeated device commands.

Device connection state.

Critical telemetry activity.

The resulting Suggested Actions / Automated Insights are displayed proactively on the Command Stream dashboard.

10. Frontend/API Integration

The frontend was updated to communicate asynchronously with the new Part 2 API functionality while preserving the existing Part 1 telemetry functionality.

Verification

The complete solution was successfully built in Visual Studio before the Part 2 implementation was committed.

Further functional verification instructions are provided in README.md.

Note

No lecturer feedback has been invented or attributed to the lecturer. Where Part 1 feedback was limited due to the strong 91% result, this document records the actual situation and focuses on the Part 2 enhancements implemented for the new requirements.