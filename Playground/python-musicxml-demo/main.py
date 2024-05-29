import os
import shutil
import zipfile
import argparse
from music21 import converter, stream, midi
from music21.musicxml.m21ToXml import ScoreExporter
from PIL import Image
import xml.etree.ElementTree as ET
import fnmatch


def remove_lyrics(score):
    for note in score.recurse().notes:
        note.lyric = None


def save_section_as_musicxml(section, file_path):
    exporter = ScoreExporter(section)
    xml_tree = exporter.parse()
    xml_string = ET.tostring(xml_tree, encoding='unicode', method='xml')
    with open(file_path, 'w', encoding='utf-8') as f:
        f.write(xml_string)


def convert_to_midi(section, midi_path):
    mf = midi.translate.music21ObjectToMidiFile(section)
    mf.open(midi_path, 'wb')
    mf.write()
    mf.close()


def convert_to_png(section, png_path):
    png_file_path = f'{png_path}.png'
    section.write('musicxml.png', fp=png_file_path)
    return png_file_path


def crop_image(image_dir, crop_height):
    image_files = fnmatch.filter(os.listdir(image_dir), 'notes*.png')

    if image_files:
        for image_file in image_files:
            full_image_path = os.path.join(image_dir, image_file)

            with Image.open(full_image_path) as img:
                width, height = img.size
                cropped_img = img.crop((0, crop_height, width, height))
                cropped_img.save(os.path.join(image_dir, 'notes.png'))

            os.remove(full_image_path)


def create_zip_file(output_zip_path, sections_dir, full_midi_path, metadata_content):
    with zipfile.ZipFile(output_zip_path, 'w') as zipf:
        zipf.write(full_midi_path, 'score.midi')
        for root, _, files in os.walk(sections_dir):
            for file in files:
                file_path = os.path.join(root, file)
                arcname = os.path.relpath(file_path, sections_dir)
                zipf.write(file_path, os.path.join('sections', arcname))

        metadata_path = os.path.join(sections_dir, 'metadata.json')
        with open(metadata_path, 'w') as f:
            f.write(metadata_content)
        zipf.write(metadata_path, 'metadata.json')


def main(input_file, output_zip, measures_per_file, crop_height, debug_mode):
    score = converter.parse(input_file)
    remove_lyrics(score)

    # Ensure all measures are numbered
    for part in score.parts:
        for i, measure in enumerate(part.getElementsByClass('Measure'), start=1):
            if measure.number is None:
                measure.number = i

    temp_dir = os.path.join(os.getcwd(), 'temp')
    if not os.path.exists(temp_dir):
        os.makedirs(temp_dir)

    full_midi_path = os.path.join(temp_dir, 'full_score.midi')
    convert_to_midi(score, full_midi_path)

    sections_dir = os.path.join(temp_dir, 'sections')
    if not os.path.exists(sections_dir):
        os.makedirs(sections_dir)

    measure_count = len(score.parts[0].measures(1, None).getElementsByClass('Measure'))
    measure_groups = [list(range(i, min(i + measures_per_file, measure_count + 1))) for i in
                      range(1, measure_count + 1, measures_per_file)]

    for idx, measure_numbers in enumerate(measure_groups, start=1):
        section = stream.Score()
        for part in score.parts:
            new_part = stream.Part()
            for measure_number in measure_numbers:
                measure = part.measure(measure_number)
                if measure is not None:
                    new_part.append(measure)
            section.append(new_part)

        section_dir = os.path.join(sections_dir, f'section_{idx}')
        os.makedirs(section_dir, exist_ok=True)

        section_musicxml_path = os.path.join(section_dir, 'section.musicxml')
        save_section_as_musicxml(section, section_musicxml_path)

        section_midi_path = os.path.join(section_dir, 'notes.midi')
        convert_to_midi(section, section_midi_path)

        section_png_path = os.path.join(section_dir, 'notes')
        convert_to_png(section, section_png_path)
        #crop_image(section_dir, crop_height)

    create_zip_file(output_zip, sections_dir, full_midi_path, '{}')

    if not debug_mode:
        shutil.rmtree(temp_dir)

    print(f"MCZIP file created successfully: {output_zip}")


if __name__ == "__main__":
    parser = argparse.ArgumentParser(
        description='Process a MusicXML file and create a zip archive with sections, MIDI, and PNG files.')
    parser.add_argument('input_file', help='Path to the input MusicXML file.')
    parser.add_argument('output_zip', help='Path to the output zip file.')
    parser.add_argument('--measures_per_file', type=int, default=10, help='Number of measures per section.')
    parser.add_argument('--crop_height', type=int, default=100,
                        help='Height of the area to crop from the top of the PNG file.')
    parser.add_argument('--debug', action='store_true', help='If set, do not delete temporary files.')

    args = parser.parse_args()
    main(args.input_file, args.output_zip, args.measures_per_file, args.crop_height, args.debug)
