# Logging Schema

## event_log.csv
Columns:
- timestamp
- event_type
- round_index
- condition_id
- object_id
- extra_1
- extra_2

Example:
- 0.16489,ROUND_START,1,C1,NA,NA,NA
- 1.81739,CARD_SELECTED,1,C1,Card_1_2,pair_3,1

## physio_log.csv
Planned columns:
- timestamp
- eda
- hr
- ibi

## questionnaire_log.csv
Planned columns:
- timestamp
- round_index
- condition_id
- question_id
- response