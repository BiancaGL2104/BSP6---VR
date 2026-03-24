# Experiment Level Events

## 1. EXPERIMENT_START
- when: participant enters VR and presses start
- data:
    - participant_id
    - timestamp

## 2. EXPERIMENT_END
- when: final questionnaire ends
- data:
    - timestamp

# Round / Condition Events

## 3. ROUND_START
- when: new condition begins
- data:
    - round_index (1-9)
    - condition_id (C1-C9)
    - visual_type (none / peripheral / text signs)
    - auditory_type (none / predictable / unpredictable)

## 4. ROUND_END
- when: round finishes
- data:
    - round_index
    - condition_id
    - completion_time

# Card Interaction Events

## 5. CARD_SELECTED
- when: user selects a card
- data:
    - card_id (e.g. Card_2_3)
    - pair_id (e.g., pair_5)
    - selection_order (1 or 2)
    - round_index
    - condition_id

## 6. MATCH
- when: correct pair
- data:
    - pair_id
    - time_since_round_start

## 7. MISMATCH
- when: incorrect pair
- data:
    - first_card_id
    - second_card_id
    - time_since_round_start

# Distractor Events

## 8. DISTRACTOR_ON
- when: distractor appears / starts
- data:
    - distractor_type (audio/visual)
    - subtype (peripheral / text / predictable / unpredictable)
    - position (left / right / upper etc.)
    - round_index

## 9. DISTRACTOR_OFF
- when: distractor stops
- data:
    - distractor_type
    - round_index

# Questionnaire Events

## 10. QUESTIONNAIRE_START
- when: after each round
- data:
    - round_index
    - condition_id

## 11. QUESTIONNAIRE_RESPONSE
- when: user answers a question
- data:
    - question_id
    - response_value (1-5 Likert)
    - round_index

## 12. QUESTIONNAIRE_END    
- when: questionnaire finishes
- data:
    - round_index

