import csv
import os
import time
from deep_translator import GoogleTranslator

# ---------------------------------------------------------
# SETTINGS
# ---------------------------------------------------------

INPUT_FILE = "localization.csv"
OUTPUT_FILE = "translations_translated.csv"

# Languages used in your CSV
LANGUAGES = {
    "spanish": "es",
    "german": "de",
    "french": "fr",
    "italian": "it",
    "russian": "ru",
    "japanese": "ja",
    "portuguese": "pt",
    "chinese": "chinese (simplified)",
    "korean": "ko"
}

# Set to True if you want to overwrite existing translations
OVERWRITE_EXISTING = False

# Delay between requests
REQUEST_DELAY = 0.2


# ---------------------------------------------------------
# TRANSLATION
# ---------------------------------------------------------

def translate_text(text, language):

    if not text.strip():
        return ""

    # Preserve literal \n sequences
    newline_token = "__NEWLINE_TOKEN__"

    text_for_translation = text.replace("\\n", newline_token)

    try:

        translator = GoogleTranslator(
            source="en",
            target=language
        )

        translated = translator.translate(text_for_translation)

        # Put the \n sequences back
        translated = translated.replace(
            newline_token,
            "\\n"
        )

        return translated

    except Exception as e:

        print(f"Retrying {language}: {e}")

        # Return original text if translation failed
        return translate_text(text, language)


# ---------------------------------------------------------
# CSV PROCESSING
# ---------------------------------------------------------

def translate_csv():

    if not os.path.exists(INPUT_FILE):
        print(f"Could not find: {INPUT_FILE}")
        return

    print("Reading CSV...")

    with open(
        INPUT_FILE,
        "r",
        encoding="utf-8",
        newline=""
    ) as infile:

        reader = csv.DictReader(infile)

        fieldnames = reader.fieldnames

        if fieldnames is None:
            print("CSV has no header.")
            return

        if "key" not in fieldnames:
            print("CSV must contain a 'key' column.")
            return

        if "english" not in fieldnames:
            print("CSV must contain an 'english' column.")
            return

        rows = list(reader)

    print(f"Found {len(rows)} translation entries.")

    # -----------------------------------------------------
    # Translate every row
    # -----------------------------------------------------

    for row_number, row in enumerate(rows, start=1):

        key = row["key"]
        english = row["english"]

        print(
            f"\n[{row_number}/{len(rows)}] {key}"
        )

        if not english.strip():
            print("  No English text. Skipping.")
            continue

        for column_name, language_code in LANGUAGES.items():

            # Make sure the column exists
            if column_name not in fieldnames:
                continue

            # Don't overwrite existing translations
            if (
                not OVERWRITE_EXISTING
                and row[column_name].strip()
            ):
                print(
                    f"  {column_name}: already translated"
                )
                continue

            print(
                f"  Translating -> {column_name}"
            )

            translated = translate_text(
                english,
                language_code
            )

            row[column_name] = translated

            time.sleep(REQUEST_DELAY)

    # -----------------------------------------------------
    # Write output
    # -----------------------------------------------------

    print("\nWriting translated CSV...")

    with open(
        OUTPUT_FILE,
        "w",
        encoding="utf-8",
        newline=""
    ) as outfile:

        writer = csv.DictWriter(
            outfile,
            fieldnames=fieldnames,
            quoting=csv.QUOTE_MINIMAL
        )

        writer.writeheader()
        writer.writerows(rows)

    print("\nDONE!")
    print(f"Output: {OUTPUT_FILE}")


# ---------------------------------------------------------
# START
# ---------------------------------------------------------

if __name__ == "__main__":
    translate_csv()
