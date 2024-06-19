import argparse
import json
import math
import subprocess
import tempfile
import zipfile
from pathlib import Path

import cv2
import numpy as np
from music21 import converter, stream, instrument

LILYPOND_PATH = 'C:\\Users\\Florian\\Downloads\\lilypond-2.24.3\\bin'
BUNDLED_PYTHON = f'{LILYPOND_PATH}\\python.exe'
INSTRUMENTS = {
    'Piano': instrument.Piano,
    'Violin': instrument.Violin,
    'Viola': instrument.Viola,
    'Flute': instrument.Flute,
    'Oboe': instrument.Oboe,
    'Clarinet': instrument.Clarinet,
    'Bassoon': instrument.Bassoon,
    'Horn': instrument.Horn,
    'Trumpet': instrument.Trumpet,
    'Trombone': instrument.Trombone,
    'Tuba': instrument.Tuba,
    'Timpani': instrument.Timpani,
    'Percussion': instrument.Percussion,
    'Harp': instrument.Harp,
    'Guitar': instrument.Guitar,
    'Organ': instrument.Organ,
    'Accordion': instrument.Accordion,
    'Harmonica': instrument.Harmonica,
    'Bagpipes': instrument.Bagpipes,
    'Banjo': instrument.Banjo,
    'Mandolin': instrument.Mandolin,
    'Sitar': instrument.Sitar,
    'Ukulele': instrument.Ukulele,
    'Koto': instrument.Koto,
    'Shamisen': instrument.Shamisen,
    'Choir': instrument.Choir
}


def parse_arguments():
    parser = argparse.ArgumentParser(description='Converts a musicxml file to a zip archive containing midi and png files.')
    parser.add_argument('input_file_path', type=Path, help='The path to the input musicxml file.')
    parser.add_argument('output_path', type=Path, help='The path to the output zip archive.')
    parser.add_argument('measures_per_line', type=int, help='The number of measures per line.')
    parser.add_argument('instrument_name', type=str, default='Piano', help='The instrument to use.')

    return parser.parse_args()


def process_section(original, measures_per_line):
    sections = []
    count = 1
    total_measures = sum(len(part.getElementsByClass('Measure')) for part in original.parts)

    for idx in range(math.ceil(total_measures / measures_per_line)):
        sections.insert(idx, stream.Score())
        sections[idx].append(original.parts[0].measures(count, count + measures_per_line - 1))
        count += measures_per_line
    return sections


def crop_image(path):
    img = cv2.imread(f'{path}\\converted.png')

    gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)

    _, binary = cv2.threshold(gray, 1, 255, cv2.THRESH_BINARY_INV)

    contours, _ = cv2.findContours(binary, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)

    min_x, min_y, max_x, max_y = np.inf, np.inf, -np.inf, -np.inf

    for contour in contours:
        x, y, w, h = cv2.boundingRect(contour)

        min_x = min(min_x, x)
        min_y = min(min_y, y)
        max_x = max(max_x, x + w)
        max_y = max(max_y, y + h)

    cropped_img = img[min_y:max_y, min_x:max_x]

    cv2.imwrite(f'{path}\\score.png', cropped_img)


def fix_ly_file(path):
    output = subprocess.run([BUNDLED_PYTHON, f'{LILYPOND_PATH}\\convert-ly.py', f'{path}\\script.ly'], shell=True, capture_output=True, text=True)

    with open(f'{path}\\converted.ly', 'w', encoding='utf8') as file:
        file.write(output.stdout)

    with open(f'{path}\\converted.ly', 'r') as file:
        data = file.read()

    data = data.replace('\\time 4/4', '\\numericTimeSignature \\time 4/4')
    data = data.replace('\\RemoveEmptyStaffContext', '\\RemoveAllEmptyStaves')
    data = data.replace("\\override VerticalAxisGroup #'remove-first = ##t",
                        "\\override VerticalAxisGroup.remove-first = ##t")

    with open(f'{path}\\converted.ly', 'w') as file:
        file.write(data)


def create_png(section, path):
    path = Path(path)
    section.write('lilypond', fp=path / 'script.ly')

    fix_ly_file(path)

    subprocess.run([f'{LILYPOND_PATH}\\lilypond.exe', "-dbackend=eps", "-dno-gs-load-fonts", "-dinclude-eps-fonts",
                    "-dresolution=300", "--format=png", "--output", path / 'converted', path / 'converted.ly'],
                   shell=True)
    crop_image(path)


def create_metadata(temp_path, measures_per_line, bpm, time_signature):
    data = {
        "measures_per_line": measures_per_line,
        "bpm": bpm,
        "time_signature": time_signature
    }

    with open(temp_path / 'metadata.json', 'w') as f:
        json.dump(data, f)


def create_archive(temp_path, output_path):
    excluded_files = ['converted.ly', 'converted.png', 'script.ly']

    with zipfile.ZipFile(output_path.with_suffix('.mczip'), 'w') as zipf:
        zipf.write(temp_path / 'score.mid', 'score.midi')
        zipf.write(temp_path / 'metadata.json', 'metadata.json')
        for file_path in temp_path.glob('sections/**/*'):
            if file_path.name in excluded_files:
                continue

            arcname = file_path.relative_to(temp_path / 'sections')
            zipf.write(file_path, Path('sections') / arcname)


def process_score(score, instrument_name):
    InstrumentClass = INSTRUMENTS.get(instrument_name.lower(), instrument.Piano)

    for part in score.parts:
        part.insert(0, InstrumentClass())

        for measure in part.getElementsByClass('Measure'):
            for n in measure.notesAndRests:
                if n.duration.quarterLength == 0:
                    measure.remove(n)
    return score


def remove_lyrics(score):
    for note in score.recurse().notes:
        note.lyric = None


def main():
    args = parse_arguments()

    orig = converter.parse(args.input_file_path)

    remove_lyrics(orig)

    orig = process_score(orig, args.instrument_name)

    sections = process_section(orig, args.measures_per_line)

    bpm = orig.metronomeMarkBoundaries()[0][2].number
    time_signature = orig.getTimeSignatures()[0].ratioString

    with tempfile.TemporaryDirectory() as temp_path:
        temp_path = Path(temp_path)
        orig.write('midi', fp=f'{temp_path}\\score.mid')
        (temp_path / 'sections').mkdir()

        for idx, section in enumerate(sections):
            section = process_score(section, args.instrument_name)

            section_path = temp_path / 'sections' / str(idx)
            section_path.mkdir()
            section.write('midi', fp=section_path / 'score.mid')
            create_png(section, section_path)

        create_metadata(temp_path, args.measures_per_line, bpm, time_signature)
        create_archive(temp_path, args.output_path)


if __name__ == '__main__':
    try:
        main()
    except Exception as e:
        print(f'An error occurred: {e}')
