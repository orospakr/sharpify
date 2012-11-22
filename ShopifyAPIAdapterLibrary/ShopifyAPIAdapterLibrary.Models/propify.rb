#!/usr/bin/env ruby

models_path = File.dirname(__FILE__)

puts "Transmogrifying any default properties to PropertySet() in: #{models_path}"

regex = /public\s([a-zA-Z\?]*)\s([a-zA-Z]*)\s{ get; set; }/

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
  cs_file_path = File.join(models_path, cs_file)

  cs_file_contents = File.read(cs_file_path)
  puts "... ... #{cs_file_contents.length}"

  
  transmog = cs_file_contents.gsub(regex, replace_with)
  puts transmog
end
