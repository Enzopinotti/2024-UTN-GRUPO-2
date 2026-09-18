import formatCamelCase from './formatCamelCase';

describe('formatCamelCase', () => {
  test('capitalizes ordinary words', () => {
    expect(formatCamelCase('frutos secos premium')).toBe('Frutos Secos Premium');
  });

  test('keeps configured connector words lowercase', () => {
    expect(formatCamelCase('frutos secos de la patagonia')).toBe(
      'Frutos Secos de la Patagonia'
    );
  });

  test('normalizes existing casing', () => {
    expect(formatCamelCase('GRANOLA Y AVENA')).toBe('Granola y Avena');
  });
});
