import { Pipe, PipeTransform } from '@angular/core';
import { AbstractControl } from '@angular/forms';

@Pipe({
  name: 'isInvalid',
})
export class IsInvalidPipe implements PipeTransform {
  transform(control: AbstractControl): any {
    return control.invalid && (control.dirty || control.touched);
  }
}
