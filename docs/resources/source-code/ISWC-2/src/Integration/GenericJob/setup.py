import setuptools
from setuptools import find_packages

setuptools.setup(
     name='generic_job',  
     version='1.0',
     author='Spanish Point Technologies',
     author_email='support@spanishpoint.ie',
     description='Generic Job Process',
     url='https://spanishpoint.ie',
     packages=find_packages(),
     classifiers=[
         'Programming Language :: Python :: 3',
         'License :: OSI Approved :: MIT License',
         'Operating System :: OS Independent',
     ],
)
