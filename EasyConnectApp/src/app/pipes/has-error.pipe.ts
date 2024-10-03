import { Pipe, PipeTransform } from '@angular/core';
import { AbstractControl } from '@angular/forms';

@Pipe({
  name: 'hasError',
  pure: false,
  standalone: true,
})
export class HasErrorPipe implements PipeTransform {
  transform(control: AbstractControl, errorName: string): any {
    const errors = control.errors || {};
    return control.invalid && (control.dirty || control.touched) && errors[errorName];
  }
}
