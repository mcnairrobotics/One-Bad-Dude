import csv
import time
import os

from deep_translator import GoogleTranslator


# =========================================================
# SETTINGS
# =========================================================

INPUT_FILE = "A_Boy_and_His_God_Localized.csv"
OUTPUT_FILE = "translation_check.txt"

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

REQUEST_DELAY = 0.2
MAX_RETRIES = 5


# =========================================================
# TRANSLATION
# =========================================================

def translate_to_english(text, source_language):

    if not text.strip():
        return ""

    # Preserve Unity's literal \n
    newline_token = "__NEWLINE_TOKEN__"

    text_for_translation = text.replace(
        "\\n",
        newline_token
    )

    for attempt in range(MAX_RETRIES):

        try:

            translator = GoogleTranslator(
                source=source_language,
                target="en"
            )

            translated = translator.translate(
                text_for_translation
            )

            if translated:

                # Restore \n
                translated = translated.replace(
                    newline_token,
                    "\\n"
                )

                return translated

        except Exception as e:

            print(
                f"    Attempt {attempt + 1}/{MAX_RETRIES} "
                f"failed: {e}"
            )

        wait_time = 2 ** attempt

        print(
            f"    Retrying in {wait_time} seconds..."
        )

        time.sleep(wait_time)

    print("    FAILED!")

    return "[TRANSLATION FAILED]"


# =========================================================
# CSV READER
# =========================================================

def read_csv():

    if not os.path.exists(INPUT_FILE):
        print(f"Could not find {INPUT_FILE}")
        return None, None

    with open(
        INPUT_FILE,
        "r",
        encoding="utf-8-sig",
        newline=""
    ) as file:

        reader = csv.DictReader(file)

        rows = list(reader)

        return reader.fieldnames, rows


# =========================================================
# CREATE REPORT
# =========================================================

def create_report():

    fieldnames, rows = read_csv()
    print(fieldnames)
    if fieldnames is None:
        return

    if "key" not in fieldnames:
        print("CSV is missing the 'key' column.")
        return

    if "english" not in fieldnames:
        print("CSV is missing the 'english' column.")
        return

    report = []

    total_translations = 0

    print(
        f"Checking {len(rows)} dialogue entries..."
    )

    # =====================================================
    # PROCESS EACH DIALOGUE
    # =====================================================

    for row_number, row in enumerate(rows, start=1):

        key = row["key"]
        english = row["english"]

        print(
            f"\n[{row_number}/{len(rows)}] {key}"
        )

        report.append("=" * 60)
        report.append(key)
        report.append("=" * 60)
        report.append("")

        report.append("ORIGINAL ENGLISH:")
        report.append(
            english.replace("\\n", "\n")
        )
        report.append("")

        # =================================================
        # CHECK EACH LANGUAGE
        # =================================================

        for language, language_code in LANGUAGES.items():

            translation = row.get(
                language,
                ""
            )

            # Nothing to check
            if not translation.strip():

                report.append(
                    f"{language.upper()}:"
                )

                report.append(
                    "[NO TRANSLATION]"
                )

                report.append("")

                continue

            total_translations += 1

            print(
                f"  Checking {language}..."
            )

            back_translation = translate_to_english(
                translation,
                language_code
            )

            report.append(
                f"{language.upper()}:"
            )

            report.append(
                translation.replace(
                    "\\n",
                    "\n"
                )
            )

            report.append("")

            report.append(
                "BACK-TRANSLATED TO ENGLISH:"
            )

            report.append(
                back_translation.replace(
                    "\\n",
                    "\n"
                )
            )

            report.append("")

            time.sleep(REQUEST_DELAY)

        report.append("")

    # =====================================================
    # WRITE REPORT
    # =====================================================

    with open(
        OUTPUT_FILE,
        "w",
        encoding="utf-8",
        newline=""
    ) as file:

        file.write(
            "\n".join(report)
        )

    print()
    print("=" * 60)
    print("CHECK COMPLETE")
    print("=" * 60)
    print()
    print(
        f"Translations checked: {total_translations}"
    )
    print(
        f"Report created: {OUTPUT_FILE}"
    )


# =========================================================
# START
# =========================================================

if __name__ == "__main__":

    create_report()