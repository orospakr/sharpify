#!/usr/bin/env ruby

# Replace simple { get; set; } flat properties with
# explicitly generated (and sadly boilerplatey) prop
# getter and setters that call SetProperty() in order
# to make our IPropertyChangedRegime work.

# Obviously a bit naiively and regexy, but it works.
# I didn't want to use Fody or a Weaver because of lack
# of MonoDevelop/xbuild support.

models_path = File.dirname(__FILE__)

puts "Transmogrifying any default properties to PropertySet() in: #{models_path}"

regex = /public\s([a-zA-Z\?\<\>]*)\s([a-zA-Z]*)\s{ get; set; }/

# match group \1 is the property type, \2 is the property name
replace_with = <<-eos
private \\1 _\\2;
        public \\1 \\2
        {
            get { return _\\2; }
            set {
                SetProperty(ref _\\2, value);
            }
        }
eos

cs_glob = File.join(models_path, "*.cs")

Dir.glob(cs_glob) do |cs_file|
  puts "... #{cs_file}"

  if cs_file =~ /Fragments\.cs/i
    puts "... skipping fragments."
    next
  end

  cs_file_path = File.join(models_path, cs_file)

  cs_file_contents = ""
  fd = File.open(cs_file_path, "rb") do |fd|
    cs_file_contents = fd.read()
  end
  
  transmog = cs_file_contents.gsub(regex, replace_with)

  fd = File.open(cs_file_path, "wb") do |fd|
    fd.write(transmog)
  end
end
